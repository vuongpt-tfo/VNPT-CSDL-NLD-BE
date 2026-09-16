# Secret handling (appsettings.*.Development.json)

> Cách áp dụng mục 9 của `git-flow.md` trong repo này: không commit giá trị thật, dùng file `.example` + `.gitignore`, chặn bằng pre-commit hook. Last reviewed: 2026-09-16.

## Quy ước

- File có giá trị thật khi dev local: `src/**/appsettings*.Development.json` — nằm trong `.gitignore` (`src/.gitignore`), **không bao giờ** commit, kể cả đã redact.
- File mẫu commit vào repo: `appsettings.Development.json.example` (cùng thư mục), chỉ chứa placeholder dạng `<ten-placeholder>`, không có giá trị thật.
- Khi checkout lần đầu: copy `.example` → bỏ đuôi `.example`, tự điền giá trị thật cục bộ, không commit.

```bash
cp src/Dashboard.API/appsettings.Development.json.example src/Dashboard.API/appsettings.Development.json
# roi tu dien Username/Password/Host that vao file appsettings.Development.json (khong commit file nay)
```

## Pre-commit hook (chặn cơ bản)

Repo hiện **chưa cài gitleaks/git-secrets thật** (máy dev chưa có sẵn công cụ này). Tạm thời có hook regex cơ bản tại `scripts/git-hooks/pre-commit`, chặn commit nếu staged diff khớp pattern connection string / password / secret dạng gán giá trị. Đây là lưới an toàn tạm thời, **không thay thế** gitleaks — khi team có gitleaks, đổi hook này sang gọi `gitleaks protect --staged`.

Cài hook (chạy 1 lần sau khi clone):

```bash
bash scripts/setup-hooks.sh
```

## TODO (hạ tầng, không tự làm được từ phía assistant)

- Bật branch protection cho `main`/`develop` trên GitHub (yêu cầu PR + review + CI, chặn push thẳng/force-push) — cần quyền admin repo.
- Thiết lập CI (build + test [+ secret scan bằng gitleaks]) chạy trên PR vào `develop`/`main`.
- Thay hook regex tạm thời bằng `gitleaks` thật khi máy dev có công cụ này.
