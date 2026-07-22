# Mouse Pointer Indicator

마우스 위치를 화면에 크게 강조 표시해 주는 Windows 트레이 앱입니다. 발표·화면 공유·강의처럼 **"지금 여기를 클릭했다"**를 청중에게 또렷하게 보여줘야 할 때 유용합니다.

가운데 버튼(휠 클릭)을 누르면 그 자리에 빨간 화살표와 **Click** 라벨이 나타납니다.

## 주요 기능

- **트레이 상주** — 실행하면 창 없이 시스템 트레이 아이콘으로 바로 들어갑니다.
- **클릭 위치 표시** — 마우스 **가운데 버튼 클릭** 시 해당 위치에 빨간 화살표 오버레이 표시.
- **화면 위/아래 자동 방향** — 커서가 화면 위쪽이면 화살표가 아래를 향하고(⌄), 아래쪽이면 위를 향하도록(⌃) 자동 전환.
- **누적 표시** — 여러 번 클릭하면 표시가 계속 쌓입니다.
- **단축키로 지우기**
  - `Esc` — 표시된 오버레이 **전체 지우기**
  - `Ctrl + Z` — **마지막 표시 하나만** 지우기

## 단축키 요약

| 동작 | 입력 |
|---|---|
| 클릭 위치 표시 | 마우스 가운데 버튼 클릭 |
| 전체 지우기 | `Esc` |
| 마지막 하나 지우기 | `Ctrl + Z` |
| 앱 종료 | 트레이 아이콘 우클릭 → **종료** |

## 요구 사항

- Windows 10 / 11
- 소스에서 빌드 시: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

> 자체 포함(single-file) 방식으로 게시한 exe는 .NET 설치 없이 실행됩니다.

## 빌드

```powershell
# 디버그 빌드
dotnet build

# 릴리스 빌드
dotnet build -c Release

# 빌드 + 실행
dotnet run
```

빌드 결과물: `bin\Debug\net8.0-windows\MousePointerIndicator.exe`

## 배포 (어디서든 실행)

.NET이 설치되지 않은 PC에서도 exe 하나만 복사해 실행할 수 있도록 자체 포함 단일 파일로 게시합니다.

```powershell
dotnet publish -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

결과물: `bin\Release\net8.0-windows\win-x64\publish\MousePointerIndicator.exe`

> 32비트 대상이 필요하면 `win-x64`를 `win-x86`으로 바꿉니다.

## 프로젝트 구조

| 파일 | 역할 |
|---|---|
| `App.xaml` / `App.xaml.cs` | 앱 진입점. 트레이 아이콘 생성, 훅 시작, 종료 처리 |
| `MouseHook.cs` | 저수준 마우스 훅 — 가운데 버튼 클릭 감지 |
| `KeyboardHook.cs` | 저수준 키보드 훅 — `Esc`, `Ctrl + Z` 감지 |
| `OverlayManager.cs` | 오버레이 창들의 표시/제거 관리 |
| `OverlayWindow.xaml` / `.xaml.cs` | 화살표 + Click 라벨 오버레이 창 |
| `ArrowDirection.cs` | 화살표 방향(Up/Down) 열거형 |
| `NativeMethods.cs` | Win32 API P/Invoke 선언 |
| `app.ico` | 앱 및 트레이 아이콘 |

## 동작 원리

Windows 저수준 훅(`WH_MOUSE_LL`, `WH_KEYBOARD_LL`)을 전역으로 설치해 마우스 가운데 클릭과 지정 키 입력을 가로챕니다. 클릭 위치에는 투명·최상위(Topmost)·클릭 통과되지 않는 작은 WPF 창을 띄워 화살표를 그립니다.
