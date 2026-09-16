using Dashboard.Application.Abstractions.Persistence;
using Dashboard.Application.Common.Dtos;
using Dashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Dashboard.Infrastructure.Persistence.Repositories;

public sealed class LabourDashboardReadRepository
    : ILabourDashboardReadRepository
{
    private readonly DashboardDbContext _context;

    public LabourDashboardReadRepository(
        DashboardDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // D01-W05 - Bar nhom tuoi
    // ============================================================
    //
    // ByAgeGroup la JSONB phu tro (map age_group_code -> so luong,
    // khong doi grain) tren labour_economic_status_summary (chuyen tu
    // labour_identity_summary.age_group_code - von la grain that -
    // sang JSONB de co provinceCode/year loc that, cung pattern voi
    // ByIndustry cua W07). Ten hien thi/thu tu van lay tu
    // dim_analytical/dim_analytical_item nhu truoc.

    public async Task<IReadOnlyList<LabourAgeDto>> GetByAgeAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken)
    {
        const string analyticalCode = "AGE_GROUP";
        const string languageCode = "vi";

        var targetPeriod = await FindLatestEconomicStatusPeriodAsync(
            provinceCode, year, cancellationToken);

        if (!targetPeriod.HasValue)
        {
            return Array.Empty<LabourAgeDto>();
        }

        var query = _context.LabourEconomicStatusSummaries
            .AsNoTracking()
            .Where(x => x.ReferencePeriod == targetPeriod.Value);

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            query = query.Where(
                x => x.AdministrativeUnitCode == provinceCode);
        }

        var byAgeGroupJsonRows = await query
            .Select(x => x.ByAgeGroup)
            .ToListAsync(cancellationToken);

        var ageGroupCounts = MergeJsonCounts(byAgeGroupJsonRows);

        if (ageGroupCounts.Count == 0)
        {
            return Array.Empty<LabourAgeDto>();
        }

        var ageGroupCodes = ageGroupCounts.Keys.ToList();

        var ageGroupMeta = await (
            from item in _context.DimAnalyticalItems.AsNoTracking()

            join analytical in _context.DimAnalyticals.AsNoTracking()
                on item.AnalyticalId equals analytical.AnalyticalId

            join translation in _context.DimAnalyticalItemTranslations.AsNoTracking()
                on item.AnalyticalItemId equals translation.AnalyticalItemId
                into translations

            from translation in translations
                .Where(x => x.LanguageCode == languageCode)
                .DefaultIfEmpty()

            where analytical.AnalyticalCode == analyticalCode
                  && analytical.IsActive
                  && item.IsActive
                  && ageGroupCodes.Contains(item.ItemCode)

            select new
            {
                item.ItemCode,
                ItemValue = translation != null
                    ? translation.ItemName
                    : item.ItemValue,
                item.SortOrder
            }
        )
        .ToListAsync(cancellationToken);

        var metaByCode = ageGroupMeta
            .ToDictionary(x => x.ItemCode, x => x);

        var total = ageGroupCounts.Values.Sum();

        return ageGroupCounts
            .Select(x => new
            {
                Code = x.Key,
                Count = x.Value,
                Meta = metaByCode.TryGetValue(x.Key, out var meta)
                    ? meta
                    : null
            })
            .OrderBy(x => x.Meta?.SortOrder ?? int.MaxValue)
            .Select(x => new LabourAgeDto
            {
                AgeRangeId = x.Meta?.SortOrder ?? 0,

                AgeRange = x.Meta?.ItemValue ?? x.Code,

                LabourCount = x.Count,

                Percentage = total == 0
                    ? 0
                    : Math.Round(
                        x.Count * 100m / total,
                        2)
            })
            .ToList();
    }

    // ============================================================
    // D01-W01/W02 - Tim ky/thang dai dien tren labour_economic_status_summary
    // ============================================================
    //
    // reference_period la semi-additive (xem
    // database/05_summary/labour_economic_status_summary.sql) - dung
    // chung cho ca GetByEconomicStatusAsync (W02) va GetKpiSummaryAsync
    // (W01, phan %tang truong theo tung nhom EMP/UNE/INA). Neu year
    // null, lay ky gan nhat hien co (= "hien tai"); neu co year, lay
    // ky gan nhat TRONG nam do.

    private async Task<DateOnly?> FindLatestEconomicStatusPeriodAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken)
    {
        var query = _context.LabourEconomicStatusSummaries
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            query = query.Where(
                x => x.AdministrativeUnitCode == provinceCode);
        }

        if (year.HasValue)
        {
            query = query.Where(
                x => x.ReferencePeriod.Year == year.Value);
        }

        return await query
            .Select(x => (DateOnly?)x.ReferencePeriod)
            .MaxAsync(cancellationToken);
    }

    // ============================================================
    // D01-W02 - Donut trang thai lao dong
    // ============================================================
    //
    // Nguon: labour_economic_status_summary JOIN
    // dim_catalog_items(catalog_name='ECONOMIC_ACTIVITY'). Theo LLD
    // (claude/d01-w02-lld-content-08-09.md): provinceCode va year deu
    // loc duoc that - year chon ky/thang gan nhat trong nam do lam dai
    // dien (xem FindLatestEconomicStatusPeriodAsync), khong SUM nhieu
    // ky lai (semi-additive).

    public async Task<IReadOnlyList<LabourEconomicStatusDto>> GetByEconomicStatusAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken)
    {
        const string catalogName = "ECONOMIC_ACTIVITY";

        var targetPeriod = await FindLatestEconomicStatusPeriodAsync(
            provinceCode, year, cancellationToken);

        if (!targetPeriod.HasValue)
        {
            return Array.Empty<LabourEconomicStatusDto>();
        }

        var query =
            from summary in _context.LabourEconomicStatusSummaries.AsNoTracking()
            join catalogItem in _context.DimCatalogItems.AsNoTracking()
                on summary.EconomicStatusCode equals catalogItem.Code
            where catalogItem.CatalogName == catalogName
                  && catalogItem.IsActive
                  && summary.ReferencePeriod == targetPeriod.Value
            select new
            {
                summary.AdministrativeUnitCode,
                summary.EconomicStatusCode,
                EconomicStatusName = catalogItem.Name,
                summary.LabourCount
            };

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            query = query.Where(
                x => x.AdministrativeUnitCode == provinceCode);
        }

        var economicStatusRows = await query
            .GroupBy(x => new
            {
                x.EconomicStatusCode,
                x.EconomicStatusName
            })
            .Select(g => new
            {
                g.Key.EconomicStatusCode,
                g.Key.EconomicStatusName,
                LabourCount = g.Sum(x => x.LabourCount)
            })
            .ToListAsync(cancellationToken);

        var economicStatusTotal = economicStatusRows.Sum(x => x.LabourCount);

        return economicStatusRows
            .Select(x => new LabourEconomicStatusDto
            {
                EconomicStatusCode = x.EconomicStatusCode,
                EconomicStatusName = x.EconomicStatusName,
                LabourCount = x.LabourCount,
                Percentage = economicStatusTotal == 0
                    ? 0
                    : Math.Round(
                        x.LabourCount * 100m / economicStatusTotal,
                        2)
            })
            .ToList();
    }

    // ============================================================
    // D01-W03 - Choropleth phan bo lao dong theo tinh/thanh
    // ============================================================
    //
    // Nguon: labour_identity_summary GROUP BY administrative_unit_code,
    // JOIN dim_administrative_unit lay ten. Theo LLD
    // (claude/d01-w03-lld-content-08-09.md): 0 tham so (giong tien le
    // GetByAgeAsync), KHONG tra field bucket mau - FE tu tinh thang mau
    // tu labourCount.

    // ============================================================
    // D01-W03 - Ban do phan bo lao dong theo tinh/thanh
    // ============================================================
    //
    // Nguon: labour_economic_status_summary (co reference_period) -
    // KHONG con dung labour_identity_summary. provinceCode va year deu
    // loc duoc that: neu truyen provinceCode, CHI tra ve 1 dong (dung
    // tinh do) de FE "chi hien ban do o tinh do" - Percentage van tinh
    // tren tong CA NUOC (khong phai tong sau khi loc) de giu y nghia
    // "chiem bao nhieu % ca nuoc" dung ngay ca khi dang xem 1 tinh.

    public async Task<IReadOnlyList<LabourByProvinceDto>> GetByProvinceAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken)
    {
        var targetPeriod = await FindLatestEconomicStatusPeriodAsync(
            null, year, cancellationToken);

        if (!targetPeriod.HasValue)
        {
            return Array.Empty<LabourByProvinceDto>();
        }

        var nationalTotal = await _context.LabourEconomicStatusSummaries
            .AsNoTracking()
            .Where(x => x.ReferencePeriod == targetPeriod.Value)
            .SumAsync(x => x.LabourCount, cancellationToken);

        var query =
            from summary in _context.LabourEconomicStatusSummaries.AsNoTracking()

            join unit in _context.DimAdministrativeUnits.AsNoTracking()
                on summary.AdministrativeUnitCode equals unit.Code

            where unit.IsActive
                  && summary.ReferencePeriod == targetPeriod.Value

            select new
            {
                summary.AdministrativeUnitCode,
                unit.Code,
                unit.Name,
                summary.LabourCount
            };

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            query = query.Where(
                x => x.AdministrativeUnitCode == provinceCode);
        }

        var rows = await query
            .GroupBy(x => new
            {
                x.Code,
                x.Name
            })
            .Select(g => new
            {
                ProvinceCode = g.Key.Code,
                ProvinceName = g.Key.Name,
                LabourCount = g.Sum(x => x.LabourCount)
            })
            .ToListAsync(cancellationToken);

        return rows
            .OrderByDescending(x => x.LabourCount)
            .Select(x => new LabourByProvinceDto
            {
                ProvinceCode = x.ProvinceCode,
                ProvinceName = x.ProvinceName,
                LabourCount = x.LabourCount,
                Percentage = nationalTotal == 0
                    ? 0
                    : Math.Round(
                        x.LabourCount * 100m / nationalTotal,
                        2)
            })
            .ToList();
    }

    // ============================================================
    // D01-W04 - Pie gioi tinh
    // ============================================================
    //
    // ByGender la JSONB phu tro (map gender_code -> so luong, khong
    // doi grain) tren labour_economic_status_summary. TRUOC DAY thiet
    // ke la cot scalar MaleCount voi Nu suy ra = LabourCount -
    // MaleCount (xem claude/rnd-branch-labour-dashboard-period-doi-chieu-08-09.md
    // cho nguon goc thiet ke ban dau) - da BO cach nay vi gender_code
    // cua labour_identity_snapshot tham chieu "Catalog Gender" chua
    // tung duoc khai bao (khong co gi dam bao chi co 2 gia tri
    // MALE/FEMALE). Neu co gender_code khac (vd chua xac dinh), phep
    // tru se am tham gop ho vao FEMALE - sai. Gio doc truc tiep tung
    // gender_code that co trong du lieu, JOIN catalog GENDER
    // (MALE/FEMALE/UNKNOWN, xem dimension-init.sql) de lay ten hien
    // thi - cung pattern voi GetByEconomicStatusAsync.

    public async Task<IReadOnlyList<LabourGenderDto>> GetByGenderAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken)
    {
        const string catalogName = "GENDER";

        var targetPeriod = await FindLatestEconomicStatusPeriodAsync(
            provinceCode, year, cancellationToken);

        if (!targetPeriod.HasValue)
        {
            return Array.Empty<LabourGenderDto>();
        }

        var query = _context.LabourEconomicStatusSummaries
            .AsNoTracking()
            .Where(x => x.ReferencePeriod == targetPeriod.Value);

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            query = query.Where(
                x => x.AdministrativeUnitCode == provinceCode);
        }

        var byGenderJsonRows = await query
            .Select(x => x.ByGender)
            .ToListAsync(cancellationToken);

        var genderCounts = MergeJsonCounts(byGenderJsonRows);

        if (genderCounts.Count == 0)
        {
            return Array.Empty<LabourGenderDto>();
        }

        var genderCodes = genderCounts.Keys.ToList();

        var genderNames = await _context.DimCatalogItems
            .AsNoTracking()
            .Where(x => x.CatalogName == catalogName
                        && genderCodes.Contains(x.Code))
            .ToDictionaryAsync(
                x => x.Code,
                x => x.Name,
                cancellationToken);

        var total = genderCounts.Values.Sum();

        return genderCounts
            .Select(x => new LabourGenderDto
            {
                GenderCode = x.Key,
                GenderName = genderNames.TryGetValue(x.Key, out var name)
                    ? name
                    : x.Key,
                LabourCount = x.Value,
                Percentage = total == 0
                    ? 0
                    : Math.Round(
                        x.Value * 100m / total,
                        2)
            })
            .OrderByDescending(x => x.LabourCount)
            .ToList();
    }

    // ============================================================
    // Helper - gop nhieu JSONB dang { code: count } lai voi nhau
    // ============================================================
    //
    // Dung chung cho ByIndustry (W07) va ByAgeGroup (W05) - ca hai deu
    // la cot phu tro tren labour_economic_status_summary, khong phai
    // grain, nen phai aggregate o tang C# thay vi SQL.

    private static Dictionary<string, long> MergeJsonCounts(
        IEnumerable<string> jsonRows)
    {
        var counts = new Dictionary<string, long>();

        foreach (var json in jsonRows)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                continue;
            }

            using var document = JsonDocument.Parse(json);

            foreach (var property in document.RootElement.EnumerateObject())
            {
                var count = property.Value.GetInt64();

                counts[property.Name] =
                    counts.TryGetValue(property.Name, out var existing)
                        ? existing + count
                        : count;
            }
        }

        return counts;
    }

    // ============================================================
    // D01-W07 - Top 10 nganh
    // ============================================================
    //
    // ByIndustry la JSONB phu tro (map industry_code -> so luong,
    // khong doi grain) tren labour_economic_status_summary (chuyen tu
    // labour_identity_summary sang de co provinceCode/year loc that),
    // chi co gia tri khi EconomicStatusCode == "EMP". Mot lao dong
    // thuoc nhieu nganh se duoc cong vao nhieu key (dem lap) - Design
    // Decision can Kan xac nhan lai, xem Open Issue D01-W07.

    public async Task<IReadOnlyList<LabourIndustryDto>> GetByIndustryAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken)
    {
        const string employedStatusCode = "EMP";

        var targetPeriod = await FindLatestEconomicStatusPeriodAsync(
            provinceCode, year, cancellationToken);

        if (!targetPeriod.HasValue)
        {
            return Array.Empty<LabourIndustryDto>();
        }

        var query = _context.LabourEconomicStatusSummaries
            .AsNoTracking()
            .Where(x => x.ReferencePeriod == targetPeriod.Value
                        && x.EconomicStatusCode == employedStatusCode);

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            query = query.Where(
                x => x.AdministrativeUnitCode == provinceCode);
        }

        var byIndustryJsonRows = await query
            .Select(x => x.ByIndustry)
            .ToListAsync(cancellationToken);

        var industryCounts = MergeJsonCounts(byIndustryJsonRows);

        if (industryCounts.Count == 0)
        {
            return Array.Empty<LabourIndustryDto>();
        }

        var industryCodes = industryCounts.Keys.ToList();

        var industryNames = await _context.DimIndustries
            .AsNoTracking()
            .Where(x => industryCodes.Contains(x.Code))
            .ToDictionaryAsync(
                x => x.Code,
                x => x.Name,
                cancellationToken);

        var total = industryCounts.Values.Sum();

        return industryCounts
            .OrderByDescending(x => x.Value)
            .Take(10)
            .Select(x => new LabourIndustryDto
            {
                IndustryCode = x.Key,
                IndustryName = industryNames.TryGetValue(x.Key, out var name)
                    ? name
                    : x.Key,
                LabourCount = x.Value,
                Percentage = total == 0
                    ? 0
                    : Math.Round(
                        x.Value * 100m / total,
                        2)
            })
            .ToList();
    }


    // ============================================================
    //
    // Nguon: labour_economic_status_summary GROUP BY
    // education_level_code (chuyen tu labour_identity_summary sang de
    // co provinceCode/year loc that), JOIN
    // dim_catalog_items(catalog_name='EDUCATION_LEVEL'). Theo LLD
    // (claude/d01-w06-lld-content-09-09.md):
    //  - Open Issue #1 (QUAN TRONG NHAT, CHUA chot voi VNPT): schema co
    //    2 catalog "trinh do" song song (EDUCATION_LEVEL 7 gia tri vs
    //    TECHNICAL_EDUCATION_LEVEL 8 gia tri) - dang tam dung
    //    EDUCATION_LEVEL vi khop so luong "7 cap" nhung CHUA co xac
    //    nhan chinh thuc day la dung cot cho khai niem A06.
    //  - Open Issue #4: LEFT JOIN (khong phai INNER JOIN nhu
    //    GetByAgeAsync/GetByEconomicStatusAsync) de tranh loai am tham
    //    ban ghi co education_level_code khong khop dim_catalog_items -
    //    gop vao nhom "UNKNOWN"/"Khong xac dinh" thay vi bien mat khoi
    //    ket qua.
    //  - Sap xep theo code tang dan (dim_catalog_items khong co
    //    sort_order, khac dim_analytical_item cua W05).

    public async Task<IReadOnlyList<LabourEducationLevelDto>> GetByEducationLevelAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken)
    {
        const string catalogName = "EDUCATION_LEVEL";
        const string unknownName = "Khong xac dinh";

        var targetPeriod = await FindLatestEconomicStatusPeriodAsync(
            provinceCode, year, cancellationToken);

        if (!targetPeriod.HasValue)
        {
            return Array.Empty<LabourEducationLevelDto>();
        }

        var baseQuery = _context.LabourEconomicStatusSummaries
            .AsNoTracking()
            .Where(x => x.ReferencePeriod == targetPeriod.Value);

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            baseQuery = baseQuery.Where(
                x => x.AdministrativeUnitCode == provinceCode);
        }

        var rows = await (
            from summary in baseQuery

            join catalogItem in _context.DimCatalogItems.AsNoTracking()
                    .Where(x => x.CatalogName == catalogName && x.IsActive)
                on summary.EducationLevelCode equals catalogItem.Code
                into catalogItems

            from catalogItem in catalogItems.DefaultIfEmpty()

            group summary by new
            {
                EducationLevelCode = catalogItem != null
                    ? catalogItem.Code
                    : summary.EducationLevelCode,
                EducationLevelName = catalogItem != null
                    ? catalogItem.Name
                    : unknownName
            }
            into g

            select new
            {
                g.Key.EducationLevelCode,
                g.Key.EducationLevelName,
                LabourCount = g.Sum(x => x.LabourCount)
            }
        )
        .ToListAsync(cancellationToken);

        var total = rows.Sum(x => x.LabourCount);

        return rows
            .OrderBy(x => x.EducationLevelCode)
            .Select(x => new LabourEducationLevelDto
            {
                EducationLevelCode = x.EducationLevelCode,
                EducationLevelName = x.EducationLevelName,
                LabourCount = x.LabourCount,
                Percentage = total == 0
                    ? 0
                    : Math.Round(
                        x.LabourCount * 100m / total,
                        2)
            })
            .ToList();
    }


    // ============================================================
    //
    // Nguon: labour_economic_status_summary (co reference_period) -
    // KHONG con dung labour_identity_summary o day nua. Ly do: quyen
    // cua bo loc dung chung (Nam) phai ap dung len TOAN BO so lieu cua
    // widget co dim lien quan, khong chi rieng %tang truong - da doi
    // lai theo yeu cau (xem thao luan W01 bo loc dung chung). 4 so
    // "hien tai" gio la ky/thang GAN NHAT trong Nam da chon (hoac Nam
    // hien tai neu khong truyen) - CUNG 1 ham FindLatestEconomicStatusPeriodAsync
    // + GetEconomicStatusCountsAsync dung chung cho ca ky hien tai va
    // ky "cung ky nam truoc" (Nam - 1), dam bao nhat quan. Day la chi
    // so ton (stock) nen KHONG SUM nhieu ky - luon lay dung 1 ky dai
    // dien cho ca nam (semi-additive). Neu Nam duoc chon chua co du
    // lieu, 4 so tra ve 0 (KHONG fallback ve so "hien tai that" cua
    // labour_identity_summary de tranh hien so sai ky). KHONG dung
    // labour_time_summary o day (bang do chi phuc vu rieng W08, xem
    // GetTrendAsync). Gap G-09 (CHUA chot voi VNPT truoc go-live) van
    // ap dung cho quy tac "cung ky".

    private static decimal? CalculateGrowthPercentage(
        long current,
        long baseline)
    {
        if (baseline <= 0)
        {
            return null;
        }

        return Math.Round(
            (current - baseline) * 100m / baseline,
            2);
    }

    private async Task<(long Employed, long Unemployed, long NotInLabourForce)>
        GetEconomicStatusCountsAsync(
            string? provinceCode,
            DateOnly? period,
            CancellationToken cancellationToken)
    {
        const string employedCode = "EMP";
        const string unemployedCode = "UNE";
        const string notInLabourForceCode = "INA";

        if (!period.HasValue)
        {
            return (0, 0, 0);
        }

        var query = _context.LabourEconomicStatusSummaries
            .AsNoTracking()
            .Where(x => x.ReferencePeriod == period.Value);

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            query = query.Where(
                x => x.AdministrativeUnitCode == provinceCode);
        }

        var rows = await query
            .GroupBy(x => x.EconomicStatusCode)
            .Select(g => new
            {
                EconomicStatusCode = g.Key,
                LabourCount = g.Sum(x => x.LabourCount)
            })
            .ToListAsync(cancellationToken);

        var employed = rows
            .Where(x => x.EconomicStatusCode == employedCode)
            .Sum(x => x.LabourCount);

        var unemployed = rows
            .Where(x => x.EconomicStatusCode == unemployedCode)
            .Sum(x => x.LabourCount);

        var notInLabourForce = rows
            .Where(x => x.EconomicStatusCode == notInLabourForceCode)
            .Sum(x => x.LabourCount);

        return (employed, unemployed, notInLabourForce);
    }

    public async Task<LabourKpiSummaryDto> GetKpiSummaryAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken)
    {
        var selectedYear = year ?? DateTime.UtcNow.Year;
        var compareYear = selectedYear - 1;

        var currentPeriod = await FindLatestEconomicStatusPeriodAsync(
            provinceCode, selectedYear, cancellationToken);

        var comparePeriod = await FindLatestEconomicStatusPeriodAsync(
            provinceCode, compareYear, cancellationToken);

        var (employed, unemployed, notInLabourForce) =
            await GetEconomicStatusCountsAsync(
                provinceCode, currentPeriod, cancellationToken);

        var (compareEmployed, compareUnemployed, compareNotInLabourForce) =
            await GetEconomicStatusCountsAsync(
                provinceCode, comparePeriod, cancellationToken);

        var totalLabourCount = employed + unemployed + notInLabourForce;
        var compareTotal = compareEmployed + compareUnemployed + compareNotInLabourForce;

        return new LabourKpiSummaryDto
        {
            TotalLabourCount = totalLabourCount,
            EmployedCount = employed,
            UnemployedCount = unemployed,
            NotInLabourForceCount = notInLabourForce,
            GrowthPercentage =
                CalculateGrowthPercentage(totalLabourCount, compareTotal),
            EmployedGrowthPercentage =
                CalculateGrowthPercentage(employed, compareEmployed),
            UnemployedGrowthPercentage =
                CalculateGrowthPercentage(unemployed, compareUnemployed),
            NotInLabourForceGrowthPercentage =
                CalculateGrowthPercentage(notInLabourForce, compareNotInLabourForce)
        };
    }

    // ============================================================
    // D01-W08 - Line xu huong lao dong theo thoi gian
    // ============================================================
    //
    // Nguon: labour_time_summary, group theo reference_period (grain
    // goc con co education_level_code x technical_level_code - cong
    // don qua 2 chieu nay la hop le vi khong phai chieu thoi gian).
    // provinceCode loc duoc that (administrative_unit_code co trong
    // grain). year loc duoc that vi reference_period la DATE that,
    // khac voi year cua GetLabourByEconomicStatusQuery. IsRevised
    // cua 1 ky = true neu BAT KY dong nao cua ky do (truoc khi cong
    // don qua education/technical) da bi danh dau da dieu chinh -
    // quy tac "da dieu chinh" van la Gap G-09, chua chot voi VNPT.

    public async Task<IReadOnlyList<LabourTrendDto>> GetTrendAsync(
        string? provinceCode,
        int? year,
        CancellationToken cancellationToken)
    {
        var query = _context.LabourTimeSummaries
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            query = query.Where(
                x => x.AdministrativeUnitCode == provinceCode);
        }

        if (year.HasValue)
        {
            query = query.Where(
                x => x.ReferencePeriod.Year == year.Value);
        }

        var rows = await query
            .GroupBy(x => x.ReferencePeriod)
            .Select(g => new LabourTrendDto
            {
                ReferencePeriod = g.Key,
                WorkingAgeLabourCount = g.Sum(x => x.WorkingAgeLabourCount),
                OutWorkingAgeLabourCount = g.Sum(x => x.OutWorkingAgeLabourCount),
                TotalLabourCount = g.Sum(x => x.TotalLabourCount),
                IsRevised = g.Any(x => x.IsRevised)
            })
            .OrderBy(x => x.ReferencePeriod)
            .ToListAsync(cancellationToken);

        return rows;
    }

    // ============================================================
    // Bo loc dung chung - danh sach Nam co du lieu thuc te
    // ============================================================
    //
    // Nguon: labour_economic_status_summary.reference_period (bang
    // duy nhat gan voi bo loc Nam cua KPI/W01/W02 hien nay). Tra ve
    // giam dan (Nam moi nhat truoc) de FE mac dinh chon nam gan nhat
    // thay vi hardcode.

    public async Task<IReadOnlyList<int>> GetAvailableYearsAsync(
        CancellationToken cancellationToken)
    {
        return await _context.LabourEconomicStatusSummaries
            .AsNoTracking()
            .Select(x => x.ReferencePeriod.Year)
            .Distinct()
            .OrderByDescending(x => x)
            .ToListAsync(cancellationToken);
    }

}
