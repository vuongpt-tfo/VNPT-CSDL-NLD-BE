using Dashboard.Domain.Entities.Dimension;
using Dashboard.Domain.Entities.Dimension.Analytical;
using Dashboard.Domain.Entities.Summary;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Infrastructure.Data.Seed;

/// <summary>
/// Seed du lieu toi thieu (dimension tham chieu + vai dong Summary
/// demo) de 9 endpoint hien co cua LabourDashboardController tra ve
/// du lieu that thay vi mang rong tren moi truong Local/Development.
///
/// KHONG chay Aggregation pipeline that (khong doc Snapshot, khong
/// dung AnalyticalRuleEvaluator) - cac dong LabourEconomicStatusSummary
/// / LabourTimeSummary duoi day la SO LIEU DEMO duoc dien tay, dung de
/// dev/test UI va API contract cuc bo. KHONG dung cho bao cao chinh
/// thuc (xem docs/decisions/decision-log.md D3/D5) - cong thuc phan
/// loai EconomicStatus chinh thuc van dang cho VNPT xac nhan.
///
/// Chi chay khi Database:SeedDemoData = true (xem Program.cs) -
/// mac dinh false, chi bat trong appsettings.Local.json.
/// </summary>
public static class DashboardDemoDataSeeder
{
    private const string EmployedCode = "EMP";
    private const string UnemployedCode = "UNE";
    private const string NotInLabourForceCode = "INA";

    private static readonly DateTimeOffset SeedTimestamp =
        new(2026, 9, 16, 0, 0, 0, TimeSpan.Zero);

    public static async Task SeedAsync(
        DashboardDbContext context,
        CancellationToken cancellationToken = default)
    {
        // Guard idempotent: dung dim_administrative_unit lam marker -
        // neu da co du lieu thi khong seed lai (tranh trung lap moi
        // lan Program.cs khoi dong lai o Local/Development).
        var alreadySeeded = await context.DimAdministrativeUnits
            .AnyAsync(cancellationToken);

        if (alreadySeeded)
        {
            return;
        }

        var provinces = BuildAdministrativeUnits();
        context.DimAdministrativeUnits.AddRange(provinces);

        context.DimCatalogItems.AddRange(BuildGenderCatalog());
        context.DimCatalogItems.AddRange(BuildEconomicActivityCatalog());
        context.DimCatalogItems.AddRange(BuildEducationLevelCatalog());

        var industries = BuildIndustries();
        context.DimIndustries.AddRange(industries);

        var ageGroupItems = BuildAgeGroupAnalyticalDimension(context);

        var educationLevelCodes = new[] { "UPPER_SECONDARY", "UNIVERSITY_PLUS" };
        var provinceCodes = provinces.Select(p => p.Code).ToArray();
        var industryCodes = industries.Select(i => i.Code).ToArray();
        var ageGroupCodes = ageGroupItems.Select(i => i.ItemCode).ToArray();

        context.LabourEconomicStatusSummaries.AddRange(
            BuildEconomicStatusSummaries(
                provinceCodes, educationLevelCodes, industryCodes, ageGroupCodes));

        context.LabourTimeSummaries.AddRange(
            BuildTimeSummaries(provinceCodes, educationLevelCodes));

        await context.SaveChangesAsync(cancellationToken);
    }

    // ============================================================
    // DIM_ADMINISTRATIVE_UNIT
    // ============================================================

    private static List<DimAdministrativeUnit> BuildAdministrativeUnits()
    {
        (long Id, string Code, string Name)[] rows =
        [
            (1, "HN", "Ha Noi"),
            (2, "HCM", "TP. Ho Chi Minh"),
            (3, "DN", "Da Nang"),
            (4, "HP", "Hai Phong"),
            (5, "CT", "Can Tho"),
        ];

        return rows
            .Select(r => new DimAdministrativeUnit
            {
                AdministrativeUnitId = r.Id,
                Code = r.Code,
                Name = r.Name,
                ParentCode = null,
                UnitTypeId = null,
                IsActive = true,
                EffectiveFrom = 0,
                EffectiveTo = null,
                CreatedAt = SeedTimestamp.UtcDateTime,
                UpdatedAt = SeedTimestamp.UtcDateTime,
            })
            .ToList();
    }

    // ============================================================
    // DIM_CATALOG_ITEMS - GENDER
    // ============================================================

    private static List<DimCatalogItem> BuildGenderCatalog()
    {
        (string Code, string Name)[] rows =
        [
            ("MALE", "Nam"),
            ("FEMALE", "Nu"),
            ("UNKNOWN", "Khong xac dinh"),
        ];

        return rows
            .Select(r => NewCatalogItem("GENDER", r.Code, r.Name))
            .ToList();
    }

    // ============================================================
    // DIM_CATALOG_ITEMS - ECONOMIC_ACTIVITY
    // ============================================================

    private static List<DimCatalogItem> BuildEconomicActivityCatalog()
    {
        (string Code, string Name)[] rows =
        [
            (EmployedCode, "Co viec lam"),
            (UnemployedCode, "That nghiep"),
            (NotInLabourForceCode, "Khong tham gia hoat dong kinh te"),
        ];

        return rows
            .Select(r => NewCatalogItem("ECONOMIC_ACTIVITY", r.Code, r.Name))
            .ToList();
    }

    // ============================================================
    // DIM_CATALOG_ITEMS - EDUCATION_LEVEL (7 cap, xem Open Issue #1
    // trong LabourDashboardReadRepository.GetByEducationLevelAsync -
    // danh sach duoi day la phan loai giao duc pho thong/dao tao chuan
    // pho bien, CHUA phai xac nhan chinh thuc tu VNPT cho khai niem A06)
    // ============================================================

    private static List<DimCatalogItem> BuildEducationLevelCatalog()
    {
        (string Code, string Name)[] rows =
        [
            ("NONE", "Chua qua dao tao / Mu chu"),
            ("PRIMARY", "Tieu hoc"),
            ("LOWER_SECONDARY", "Trung hoc co so"),
            ("UPPER_SECONDARY", "Trung hoc pho thong"),
            ("VOCATIONAL", "So cap / Trung cap nghe"),
            ("COLLEGE", "Cao dang"),
            ("UNIVERSITY_PLUS", "Dai hoc tro len"),
        ];

        return rows
            .Select(r => NewCatalogItem("EDUCATION_LEVEL", r.Code, r.Name))
            .ToList();
    }

    private static DimCatalogItem NewCatalogItem(
        string catalogName, string code, string name) => new()
        {
            CatalogName = catalogName,
            Code = code,
            Name = name,
            IsActive = true,
            EffectiveFrom = 0,
            EffectiveTo = null,
            CreatedAt = SeedTimestamp.UtcDateTime,
            UpdatedAt = SeedTimestamp.UtcDateTime,
        };

    // ============================================================
    // DIM_INDUSTRY (ma nhom nganh cap 1 - tham khao muc dich demo,
    // khong phai VSIC day du)
    // ============================================================

    private static List<DimIndustry> BuildIndustries()
    {
        (long Id, string Code, string Name)[] rows =
        [
            (1, "A", "Nong, lam nghiep va thuy san"),
            (2, "C", "Cong nghiep che bien, che tao"),
            (3, "F", "Xay dung"),
            (4, "G", "Ban buon va ban le"),
            (5, "I", "Dich vu luu tru va an uong"),
        ];

        return rows
            .Select(r => new DimIndustry
            {
                IndustryId = r.Id,
                Code = r.Code,
                Name = r.Name,
                ParentCode = null,
                Level = 1,
                IsActive = true,
                EffectiveFrom = 0,
                EffectiveTo = null,
                CreatedAt = SeedTimestamp.UtcDateTime,
                UpdatedAt = SeedTimestamp.UtcDateTime,
            })
            .ToList();
    }

    // ============================================================
    // DIM_ANALYTICAL / DIM_ANALYTICAL_ITEM - AGE_GROUP
    // ============================================================
    //
    // Chi seed dinh nghia dimension + item + ban dich (dung cho
    // GetByAgeAsync JOIN lay ten hien thi). KHONG seed
    // DimAnalyticalRule/DimAnalyticalRuleCondition o day - viec phan
    // loai tuoi that (chay qua AnalyticalRuleEvaluator trong
    // LabourAggregationService) nam ngoai pham vi seed toi thieu nay;
    // cac dong LabourEconomicStatusSummary demo phia duoi tu gan san
    // ByAgeGroup, khong can chay rule engine.

    private static List<DimAnalyticalItem> BuildAgeGroupAnalyticalDimension(
        DashboardDbContext context)
    {
        var analytical = new DimAnalytical
        {
            AnalyticalCode = "AGE_GROUP",
            Name = "Nhom tuoi",
            Description = "Phan nhom tuoi phuc vu phan tich luc luong lao dong",
            DimensionType = "RANGE",
            Version = 1,
            EffectiveFrom = new DateOnly(2026, 1, 1),
            EffectiveTo = null,
            IsActive = true,
            CreatedAt = SeedTimestamp,
            UpdatedAt = SeedTimestamp,
        };

        context.DimAnalyticals.Add(analytical);

        (string Code, string Value, int Sort, decimal From, decimal? To)[] rows =
        [
            ("AGE_15_19", "15-19", 1, 15, 20),
            ("AGE_20_24", "20-24", 2, 20, 25),
            ("AGE_25_29", "25-29", 3, 25, 30),
            ("AGE_30_34", "30-34", 4, 30, 35),
            ("AGE_35_39", "35-39", 5, 35, 40),
            ("AGE_40_49", "40-49", 6, 40, 50),
            ("AGE_50_59", "50-59", 7, 50, 60),
            ("AGE_60_64", "60-64", 8, 60, 65),
            ("AGE_65_PLUS", "65+", 9, 65, null),
        ];

        var items = new List<DimAnalyticalItem>();

        foreach (var row in rows)
        {
            var item = new DimAnalyticalItem
            {
                Analytical = analytical,
                ItemCode = row.Code,
                ItemValue = row.Value,
                SortOrder = row.Sort,
                RangeFrom = row.From,
                RangeTo = row.To,
                IsActive = true,
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp,
            };

            context.DimAnalyticalItems.Add(item);

            context.DimAnalyticalItemTranslations.Add(
                new DimAnalyticalItemTranslation
                {
                    AnalyticalItem = item,
                    LanguageCode = "vi",
                    ItemName = row.Value + " tuoi",
                    CreatedAt = SeedTimestamp,
                    UpdatedAt = SeedTimestamp,
                });

            items.Add(item);
        }

        return items;
    }

    // ============================================================
    // LABOUR_ECONOMIC_STATUS_SUMMARY (demo)
    // ============================================================
    //
    // Grain: (reference_period, administrative_unit_code,
    // education_level_code, technical_level_code, economic_status_code).
    // technical_level_code chua co catalog rieng va khong duoc
    // Repository query nao dung lam JOIN key hien tai - dung hang so
    // "ALL" cho toan bo dong demo.
    //
    // 2 ky (2026-06-01 "hien tai", 2025-06-01 "cung ky nam truoc" -
    // GetKpiSummaryAsync mac dinh selectedYear = DateTime.UtcNow.Year)
    // x 2 tinh x 2 trinh do x 3 trang thai kinh te = 24 dong.

    private static List<LabourEconomicStatusSummary> BuildEconomicStatusSummaries(
        string[] provinceCodes,
        string[] educationLevelCodes,
        string[] industryCodes,
        string[] ageGroupCodes)
    {
        DateOnly[] periods = [new(2026, 6, 1), new(2025, 6, 1)];
        const string technicalLevelCode = "ALL";

        var rows = new List<LabourEconomicStatusSummary>();

        foreach (var period in periods)
        {
            // He so tang truong don gian giua 2 ky de KPI %tang truong
            // (D01-W01) co gia tri khac 0 - thuan demo, khong phai so
            // lieu that.
            var growthFactor = period.Year == 2026 ? 1.00m : 0.92m;

            foreach (var provinceCode in provinceCodes)
            {
                // He so quy mo theo tinh de cac tinh khong giong het
                // nhau tren bieu do (D01-W03).
                var provinceScale = provinceCode switch
                {
                    "HN" => 1.00m,
                    "HCM" => 1.20m,
                    "DN" => 0.35m,
                    "HP" => 0.30m,
                    "CT" => 0.25m,
                    _ => 0.20m,
                };

                foreach (var educationLevelCode in educationLevelCodes)
                {
                    var educationScale =
                        educationLevelCode == "UNIVERSITY_PLUS" ? 0.8m : 1.0m;

                    foreach (var economicStatusCode in
                        new[] { EmployedCode, UnemployedCode, NotInLabourForceCode })
                    {
                        var baseCount = economicStatusCode switch
                        {
                            EmployedCode => 8000m,
                            UnemployedCode => 400m,
                            _ => 2200m,
                        };

                        var labourCount = (long)Math.Round(
                            baseCount * provinceScale * educationScale * growthFactor);

                        rows.Add(new LabourEconomicStatusSummary
                        {
                            ReferencePeriod = period,
                            AdministrativeUnitCode = provinceCode,
                            EducationLevelCode = educationLevelCode,
                            TechnicalLevelCode = technicalLevelCode,
                            EconomicStatusCode = economicStatusCode,
                            LabourCount = labourCount,
                            ByPriority = "{}",
                            ByGender = SplitTwoWay(labourCount, "MALE", "FEMALE", 0.51m),
                            ByAgeGroup = SplitAcrossAgeGroups(labourCount, ageGroupCodes),
                            ByIndustry = economicStatusCode == EmployedCode
                                ? SplitAcrossIndustries(labourCount, industryCodes)
                                : "{}",
                            SourceIdentitySnapshotVersion = 1,
                            SourceEmploymentSnapshotVersion = 1,
                            AnalyticalDefinitionVersion = null,
                            ProcessingRunId = null,
                            AggregatedAt = SeedTimestamp,
                        });
                    }
                }
            }
        }

        return rows;
    }

    // ============================================================
    // LABOUR_TIME_SUMMARY (demo) - D01-W08
    // ============================================================
    //
    // Grain: (reference_period, administrative_unit_code,
    // education_level_code, technical_level_code). 6 ky lien tiep
    // (2026-01 .. 2026-06) x 2 tinh, education_level_code = "ALL"
    // (khong tach theo trinh do o line chart nay).

    private static List<LabourTimeSummary> BuildTimeSummaries(
        string[] provinceCodes,
        string[] educationLevelCodes)
    {
        _ = educationLevelCodes;
        const string educationLevelCode = "ALL";
        const string technicalLevelCode = "ALL";
        const short workingAgeFrom = 15;
        const short workingAgeTo = 62;

        var rows = new List<LabourTimeSummary>();

        var months = Enumerable.Range(1, 6)
            .Select(m => new DateOnly(2026, m, 1))
            .ToArray();

        foreach (var provinceCode in provinceCodes)
        {
            var provinceScale = provinceCode switch
            {
                "HN" => 1.00m,
                "HCM" => 1.20m,
                "DN" => 0.35m,
                "HP" => 0.30m,
                "CT" => 0.25m,
                _ => 0.20m,
            };

            for (var i = 0; i < months.Length; i++)
            {
                // Xu huong tang nhe qua tung thang - thuan demo.
                var monthGrowth = 1.0m + (i * 0.01m);

                var workingAge = (long)Math.Round(
                    9500m * provinceScale * monthGrowth);

                var outWorkingAge = (long)Math.Round(
                    1500m * provinceScale * monthGrowth);

                rows.Add(new LabourTimeSummary
                {
                    ReferencePeriod = months[i],
                    AdministrativeUnitCode = provinceCode,
                    EducationLevelCode = educationLevelCode,
                    TechnicalLevelCode = technicalLevelCode,
                    WorkingAgeLabourCount = workingAge,
                    OutWorkingAgeLabourCount = outWorkingAge,
                    TotalLabourCount = workingAge + outWorkingAge,
                    PreviousWorkingAgeLabourCount = null,
                    PreviousOutWorkingAgeLabourCount = null,
                    PreviousTotalLabourCount = null,
                    IsRevised = false,
                    RevisedAt = null,
                    AnalyticalDefinitionVersion = null,
                    WorkingAgeFrom = workingAgeFrom,
                    WorkingAgeTo = workingAgeTo,
                    SourceIdentitySnapshotVersion = 1,
                    SourceEmploymentSnapshotVersion = 1,
                    ProcessingRunId = null,
                    AggregatedAt = SeedTimestamp,
                });
            }
        }

        return rows;
    }

    // ============================================================
    // HELPER - phan bo LabourCount vao cac key JSONB, tong dung bang
    // LabourCount (sai lech duoc don het vao key cuoi cung).
    // ============================================================

    private static string SplitTwoWay(
        long total, string keyA, string keyB, decimal shareA)
    {
        var valueA = (long)Math.Round(total * shareA);
        var valueB = total - valueA;

        return System.Text.Json.JsonSerializer.Serialize(
            new Dictionary<string, long> { [keyA] = valueA, [keyB] = valueB });
    }

    private static string SplitAcrossAgeGroups(long total, string[] ageGroupCodes)
    {
        return SplitEvenly(total, ageGroupCodes);
    }

    private static string SplitAcrossIndustries(long total, string[] industryCodes)
    {
        return SplitEvenly(total, industryCodes);
    }

    private static string SplitEvenly(long total, string[] keys)
    {
        if (keys.Length == 0)
        {
            return "{}";
        }

        var counts = new Dictionary<string, long>();
        var remaining = total;

        for (var i = 0; i < keys.Length; i++)
        {
            if (i == keys.Length - 1)
            {
                counts[keys[i]] = remaining;
            }
            else
            {
                var share = total / keys.Length;
                counts[keys[i]] = share;
                remaining -= share;
            }
        }

        return System.Text.Json.JsonSerializer.Serialize(counts);
    }
}
