# Git Flow nội bộ

> Quy ước nhánh/commit/PR nội bộ TFO cho dự án VNPT SIMB_CSDL Người lao động — nguồn: tài liệu chính thức do VNPT/TFO ban hành, chép nguyên nội dung vào đây để cả team và AI-assistant cùng một nguồn tham chiếu. Last reviewed: 2026-09-16.

## 1. Mục đích và phạm vi

Chuẩn hoá cách team làm việc với Git trong suốt vòng đời phát triển D01 và các dashboard tiếp theo, đảm bảo:

- Code luôn có một bản build được, test được trên môi trường Dev nội bộ (`develop`) và một bản đã qua kiểm thử SIT/UAT nội bộ, sẵn sàng bàn giao (`main`).
- Mọi thay đổi đều truy vết được về đúng task trong WBS (D01-01, D01-02...) phục vụ đối soát tiến độ.

**Lưu ý quan trọng**: `main`/`develop` trong tài liệu này là nhánh trong repo của TFO. Nhánh `develop` mà VNPT cấp là một repo/đích khác, không trùng vị trí. Khi bàn giao, merge `main` (tại thời điểm bàn giao) của TFO vào `develop` của VNPT, không phải đồng bộ tên nhánh 1-1.

## 2. Sơ đồ tổng quan

`feature/*`, `fix/*`, `refactor/*`, `chore/*` → PR → `develop` → PR → `main` → MR bàn giao → `develop` (repo VNPT). `hotfix/*` rẽ thẳng từ `main`, merge vào `main` rồi back-merge vào `develop`.

## 3. Nhánh dài hạn

### 3.1 main

- Là nhánh chạy môi trường SIT/UAT nội bộ TFO.
- Là nguồn duy nhất để tạo merge request bàn giao sang nhánh `develop` của VNPT tại mỗi mốc milestone.
- Được bảo vệ (branch protection): không push thẳng, không force-push, bắt buộc có PR, có review và phải CI pass.
- Chỉ nhận code qua PR từ `develop`, hoặc `hotfix` merge thẳng khi khẩn cấp.

### 3.2 develop

- Là nhánh tích hợp hàng ngày, chạy môi trường Dev nội bộ.
- Nhận code từ các nhánh feature, fix, refactor,... qua PR.
- Khi một cụm task đã hoàn thành và unit test pass, tạo PR `develop` → `main`.
- Sau khi hotfix được merge vào `main`, luôn back-merge `main` sang `develop` ngay để không bị mất fix ở lần merge kế tiếp.

## 4. Nhánh ngắn hạn (nhánh của dev)

Quy ước đặt tên: viết thường toàn bộ, gắn mã task WBS.

```
<loại>/<mã-task>-<mô-tả-ngắn-không-dấu>
```

| Loại | Dùng khi | Merge vào | Ví dụ |
|---|---|---|---|
| `feature/` | Xây mới chức năng/API/UI | `develop` | `feature/d01-01-kpi-card-api` |
| `fix/` | Sửa lỗi phát hiện trong quá trình dev/test | `develop` | `fix/d01-02-donut-null-value` |
| `refactor/` | Tái cấu trúc code, không đổi behavior | `develop` | `refactor/d01-03-cleanup-repository` |
| `chore/` | Việc kỹ thuật không thuộc nghiệp vụ (sync template, cấu hình CI...) | `develop` | `chore/sync-vnpt-template-07-09` |
| `hotfix/` | Lỗi khẩn cấp trên `main` (đã bàn giao/đang SIT-UAT) | `main` (rồi back-merge `develop`) | `hotfix/d01-01-fix-negative-kpi` |

Quy tắc:

- Mỗi nhánh chỉ nên sống 1–3 ngày, tương ứng 1 task/sub-task cụ thể trong WBS.
- Luôn tạo nhánh mới từ `develop` mới nhất (`git pull origin develop` trước khi `checkout -b`).
- Không đặt tên chung chung như `feature/fix-bug` — phải có mã task để trace ngược lại estimate và đối soát dữ liệu.

## 5. Quy ước commit message

Dùng Conventional Commits, kèm mã task trong ngoặc:

```
<type>(<mã-task>): <mô tả ngắn, tiếng Việt không dấu hoặc tiếng Anh đều được, nhất quán trong 1 PR>
```

`type` hợp lệ: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`, `perf`, `style`.

Ví dụ:

```
feat(D01-02): them API donut chart lao dong theo gioi tinh
fix(D01-01): sua sai lech mau so KPI card
chore(D01): sync template ban cap nhat 07-09
```

## 6. Quy trình làm việc hàng ngày (feature → develop)

1. `git checkout develop && git pull`
2. `git checkout -b feature/d01-0x-mo-ta`
3. Code + tự viết/chạy unit test.
4. Commit theo quy ước ở mục 5, push nhánh.
5. Tạo Pull Request vào `develop`. PR mô tả cần nêu: mã task, tóm tắt thay đổi, cách test.
6. Tối thiểu 1 reviewer duyệt (bất kỳ dev nào trong team, khuyến khích cross-review giữa BE/FE nếu đổi API contract).
7. CI phải pass (build + lint + unit test) trước khi merge.
8. Merge bằng **squash merge** để lịch sử `develop` gọn theo từng task.
9. Xoá nhánh feature sau khi merge.

## 7. Quy trình lên main

Khi một cụm task/sub-task (vd. D01-01) đã hoàn thành trên `develop` và ổn định:

1. Tạo PR `develop` → `main`.
2. Các yêu cầu bắt buộc trước khi merge: CI build + toàn bộ test suite pass; quét secret (gitleaks/git-secrets).
3. Reviewer bắt buộc.
4. Merge (**không squash** để giữ lịch sử merge, biết `main` gồm những PR nào từ `develop`).
5. Deploy tự động/thủ công lên môi trường SIT/UAT nội bộ TFO để tự kiểm thử.

## 8. Hotfix

Dùng khi phát hiện lỗi khẩn cấp trên `main` (đang chạy SIT/UAT hoặc đã bàn giao) trong lúc team vẫn đang phát triển milestone tiếp theo trên `develop`.

1. `git checkout -b hotfix/mo-ta` từ `main`.
2. Fix + test, PR thẳng vào `main`, review bởi SA/TL.
3. Merge vào `main`.
4. Bắt buộc back-merge `main` vào `develop` ngay sau đó để fix không bị mất ở lần merge `develop` → `main` kế tiếp.

## 9. Bảo mật (secret handling)

- Không commit connection string, password, API key, JWT secret dưới bất kỳ hình thức nào — kể cả trong file `appsettings.*.Development.json`.
- Dùng file mẫu `appsettings.Development.json.example` (không chứa giá trị thật), file thật nằm trong `.gitignore`.
- Cài pre-commit hook quét secret (gitleaks hoặc git-secrets) chặn commit nếu phát hiện pattern connection string/password.

Chi tiết cách áp dụng trong repo này: xem `secret-handling.md`.

## 10. Branch protection & quyền review

| Nhánh | Push thẳng | Số reviewer tối thiểu | Reviewer bắt buộc | CI bắt buộc |
|---|---|---|---|---|
| `main` | Không | 1 | SA/TL | Có (build + test + secret scan) |
| `develop` | Không | 1 | Bất kỳ dev trong team | Có (build + test) |
| `feature/fix/refactor/chore` | Có (nhánh cá nhân) | – | – | Không bắt buộc, khuyến khích chạy local |
| `hotfix` | Không | 1 | SA/TL | Có |

## 11. Checklist trước khi tạo MR sang VNPT

- Tất cả PR liên quan milestone đã merge vào `develop` rồi vào `main`.
- SIT/UAT nội bộ TFO không còn bug Critical/High mở.
- Đã quét secret, không còn credential trong code/lịch sử git (đặc biệt lần đầu bàn giao).
- MR có mô tả đầy đủ: phạm vi, task đã hoàn thành, gap/blocker còn mở, link tài liệu liên quan (SRS/QnA).
- Đã thông báo VNPT trước khi tạo MR.

## 12. Tình trạng áp dụng trong repo `VNPT-CSDL-NLD-BE` (ghi chú của assistant)

Xem `docs/decisions/decision-log.md` (D4) cho việc áp dụng tài liệu này vào repo — bao gồm những gì đã làm được ngay (naming nhánh/commit, `.example` + `.gitignore` cho secret) và những gì còn phụ thuộc hạ tầng chưa thiết lập (branch protection, CI, pre-commit gitleaks thật — repo hiện chưa có CI/gitleaks, chỉ có hook chặn pattern cơ bản tại `scripts/git-hooks/pre-commit`).
