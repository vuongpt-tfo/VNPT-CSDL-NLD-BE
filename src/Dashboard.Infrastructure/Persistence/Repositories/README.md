# LabourDashboardReadRepository

## Fix build: thiếu `using System.Text.Json;`

`GetByIndustryAsync` dùng `JsonDocument.Parse(...)` để đọc cột JSONB
`by_industry` nhưng file thiếu `using System.Text.Json;` -- khiến toàn
bộ `Dashboard.Infrastructure` (và mọi project phụ thuộc nó) không
build được. Đã thêm import, không đổi logic gì khác.

## KHÔNG phải bug: `male_count` / `by_industry` "không tồn tại"

Nếu chạy `dotnet run` mà `GetByGenderAsync`/`GetByIndustryAsync` báo
lỗi Postgres `column "male_count"/"by_industry" does not exist`,
**đừng sửa code** -- đây không phải sai sót trong
`LabourIdentitySummaryConfiguration.cs`/entity/SQL script (3 chỗ đó
đều đã khớp nhau, đều khai báo đúng 2 cột này). Nguyên nhân thực tế:
DB local (`dashboard_local`) được tạo TRƯỚC khi
`database/05_summary/labour_identity_summary.sql` được cập nhật thêm
2 cột D01-W04/D01-W07 (09/09/2026) -- tức là DB local bị lệch so với
schema đã track trong repo, không phải code sai.

**Cách fix đúng**: đồng bộ lại DB local theo script hiện tại, KHÔNG
sửa code backend:

```bash
psql -U <user> -d <db> -c "DROP TABLE dashboard_sch.labour_identity_summary CASCADE;"
psql -U <user> -d <db> -f database/05_summary/labour_identity_summary.sql
```

(An toàn để DROP vì đây là bảng summary derive được từ snapshot, không
phải bảng nguồn -- nhưng vẫn nên kiểm tra bảng đang trống/là dữ liệu
test trước khi DROP trên môi trường không phải local của mình.)

## Dữ liệu test cục bộ

Nếu cần dữ liệu để test UI/API thủ công, DB local dùng ở đợt triển
khai FE (09-10/09/2026) được seed bằng script tự viết (không nằm
trong repo, chỉ tồn tại trong scratchpad phiên làm việc đó) -- sinh dữ
liệu giả bằng `hashtext()` cho toàn bộ 34 tỉnh/thành theo
`vn-provinces-2025.geojson` (xem
`frontend/dashboard-web/src/assets/geo/README.md`). Không có script
seed nào được commit vào repo; nếu cần lại, phải viết mới dựa theo
grain của `labour_identity_summary` (xem comment "BUSINESS GRAIN"
trong `LabourIdentitySummaryConfiguration.cs`).
