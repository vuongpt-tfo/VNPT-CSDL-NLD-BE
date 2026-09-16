using Dashboard.Application.Common.Dtos;
using MediatR;

namespace Dashboard.Application.Features.LabourDashboard.Queries.GetLabourKpiSummary;

// Chua co LLD content doc chinh thuc rieng cho W01 tinh den 09/09/2026
// (khac W02/W03/W06 da co claude/d01-w0x-lld-content-*.md duoc Kan
// duyet) - thiet ke nay dua tren claude/d01-fact-dim-doi-chieu-widget-08-09.md.
// Nguyen tac: "quyen cua bo loc dung chung ap dung len TOAN BO so lieu
// cua widget co dim lien quan" - Year o day KHONG chi anh huong
// %tang truong ma anh huong ca 4 so chinh (Tong/EMP/UNE/INA), vi
// labour_economic_status_summary da co reference_period. "Cung ky" =
// Nam - 1 (grain nho nhat cua bo loc la Nam, khong phai thang). Neu
// Year null, mac dinh Nam hien tai (theo dong ho server). Xem
// GetKpiSummaryAsync. Quy tac "cung ky nam truoc" van la placeholder
// (Gap G-09) - can Kan/VNPT xac nhan lai contract nay khi viet LLD
// chinh thuc cho W01.
public sealed record GetLabourKpiSummaryQuery(
    string? ProvinceCode,
    int? Year)
    : IRequest<LabourKpiSummaryDto>;
