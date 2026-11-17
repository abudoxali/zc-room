# Zewail City Launcher

لانشر Windows لنسخة V19 المعتمدة من لعبة مدينة زويل ثلاثية الأبعاد.

1. نزّل `ZewailCityLauncher.exe` من صفحة الإصدارات وشغّله.
2. اضغط «تثبيت اللانشر» لإضافته لقائمة ابدأ، ثم «تنزيل اللعبة». التنزيل الأول حوالي 480 ميجابايت ويحتاج اتصالاً بالإنترنت ومساحة فارغة إضافية أثناء التجهيز.
3. اضغط «العب». بعد اكتمال التنزيل تعمل اللعبة دون إنترنت ودون تثبيت Godot.

اللانشر واللعبة في `%LOCALAPPDATA%\ZewailCity`. للحذف، احذف المجلد واختصار قائمة ابدأ. ملفات حفظ اللعبة، إن وجدت، مستقلة عن مجلد التثبيت.

تحتاج اللعبة Windows 64-bit وبطاقة رسوميات متوافقة مع Godot Forward+. يختلف الأداء حسب الجهاز. الملف التنفيذي غير موقّع رقميًا، وقد يظهر Windows SmartScreen تحذيرًا.

## Build

Run `build.ps1` on Windows with .NET Framework 4.x. The launcher pins the release archive and executable SHA-256 and validates both before installation. The V19 archive contains the game and notices; its source is in [3D_ZewailCity](https://github.com/abudoxali/3D_ZewailCity). The version is intentionally pinned; updating it requires a new launcher release with updated hashes.
