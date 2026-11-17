using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

internal static class Program {
    const string Url = "https://github.com/abudoxali/zc-room/releases/download/v19.0.0/ZewailCity-V19-Windows.zip";
    const string ArchiveHash = "b8d0cf8b434953fbbcf69ecf9288bb2a0292863021f2ca54c3e92902ccff47ba";
    const string GameHash = "b3a55e6ec414fe67288cb59059ecbce56151d278611179bd2159b6672cb386b4";
    static readonly string Root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZewailCity");
    static string Hash(string path) { using (var s = File.OpenRead(path)) using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(s)).Replace("-", "").ToLowerInvariant(); }
    static string Game { get { return Path.Combine(Root, "game-v19", "ZewailCityExplorer.exe"); } }
    static void Extract(string archive, string destination) {
        if (Hash(archive) != ArchiveHash) throw new InvalidDataException("فشل التحقق من سلامة ملف اللعبة.");
        Directory.CreateDirectory(destination);
        ZipFile.ExtractToDirectory(archive, destination);
        if (Hash(Path.Combine(destination, "ZewailCityExplorer.exe")) != GameHash) throw new InvalidDataException("فشل التحقق من ملف تشغيل اللعبة.");
    }
    [STAThread] static void Main(string[] args) {
        if (args.Length == 3 && args[0] == "--verify-package") { Extract(args[1], args[2]); return; }
        Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Launcher());
    }
    sealed class Launcher : Form {
        readonly Label status = new Label(); readonly ProgressBar progress = new ProgressBar(); readonly Button action = new Button(); readonly Button install = new Button();
        public Launcher() {
            Text = "Zewail City Launcher"; ClientSize = new Size(520, 245); FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false; StartPosition = FormStartPosition.CenterScreen; Font = new Font("Segoe UI", 10); RightToLeft = RightToLeft.Yes; RightToLeftLayout = true;
            var title = new Label { Text = "مدينة زويل ثلاثية الأبعاد", Font = new Font("Segoe UI", 17, FontStyle.Bold), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Bounds = new Rectangle(15, 12, 490, 45) };
            status.Bounds = new Rectangle(22, 63, 476, 50); status.TextAlign = ContentAlignment.MiddleCenter;
            progress.Bounds = new Rectangle(30, 121, 460, 18);
            action.Bounds = new Rectangle(270, 163, 205, 45); install.Bounds = new Rectangle(45, 163, 205, 45);
            install.Text = "تثبيت اللانشر"; install.Click += Install;
            action.Click += RunGame; Controls.AddRange(new Control[] { title, status, progress, action, install }); RefreshState();
        }
        void RefreshState() { bool exists = File.Exists(Game); action.Text = exists ? "العب" : "تنزيل اللعبة"; status.Text = exists ? "اللعبة جاهزة للعب بدون إنترنت." : "نزّل اللعبة مرة واحدة (حوالي 480 ميجابايت)."; }
        void Install(object sender, EventArgs e) {
            try {
                Directory.CreateDirectory(Root); string target = Path.Combine(Root, "ZewailCityLauncher.exe");
                if (!String.Equals(Application.ExecutablePath, target, StringComparison.OrdinalIgnoreCase)) File.Copy(Application.ExecutablePath, target, true);
                Type shellType = Type.GetTypeFromProgID("WScript.Shell"); dynamic shell = Activator.CreateInstance(shellType);
                string start = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Zewail City.lnk");
                dynamic shortcut = shell.CreateShortcut(start); shortcut.TargetPath = target; shortcut.WorkingDirectory = Root; shortcut.Save();
                MessageBox.Show("تم تثبيت اللانشر وإضافة اختصار إلى قائمة ابدأ.", "تم التثبيت");
            } catch (Exception ex) { MessageBox.Show(ex.Message, "تعذر تثبيت اللانشر"); }
        }
        async void RunGame(object sender, EventArgs e) {
            if (File.Exists(Game)) { try { Process.Start(new ProcessStartInfo(Game) { WorkingDirectory = Path.GetDirectoryName(Game) }); } catch (Exception ex) { MessageBox.Show(ex.Message, "تعذر تشغيل اللعبة"); } return; }
            action.Enabled = false; install.Enabled = false; string temp = Path.Combine(Root, "download-" + Guid.NewGuid().ToString("N"));
            try {
                Directory.CreateDirectory(temp); string archive = Path.Combine(temp, "game.zip"); string extracted = Path.Combine(temp, "contents");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (var client = new WebClient()) { client.DownloadProgressChanged += (s, p) => { progress.Value = p.ProgressPercentage; status.Text = "جاري تنزيل اللعبة: " + p.ProgressPercentage + "%"; }; await client.DownloadFileTaskAsync(new Uri(Url), archive); }
                status.Text = "جاري التحقق من اللعبة وتجهيزها...";
                await Task.Run(() => Extract(archive, extracted));
                Directory.Move(extracted, Path.Combine(Root, "game-v19")); progress.Value = 100; RefreshState();
                MessageBox.Show("اكتمل التنزيل. اضغط العب لبدء اللعبة.", "جاهز");
            } catch (Exception ex) { MessageBox.Show(ex.Message, "تعذر تنزيل اللعبة"); RefreshState(); }
            finally { try { if (Directory.Exists(temp)) Directory.Delete(temp, true); } catch { } action.Enabled = true; install.Enabled = true; }
        }
    }
}
