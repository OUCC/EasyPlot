using System.Runtime.InteropServices;

namespace EasyPlot.Win32Api;

internal static partial class MessageBox
{
    public static partial class NativeMethods
    {
        [LibraryImport("User32.dll", EntryPoint = "MessageBoxW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial MBResult MessageBox(IntPtr hWnd, string text, string caption, uint type);
    }

    /// <summary>
    /// 指定したテキストとキャプションを表示するメッセージ ボックスを表示します。
    /// </summary>
    /// <param name="text">メッセージ ボックスに表示するテキスト。</param>
    /// <param name="caption">メッセージ ボックスのタイトル バーに表示するテキスト。</param>
    public static MBResult Show(string text, string caption)
    {
        return NativeMethods.MessageBox(0, text, caption, MBType.Ok | MBIcon.Error);
    }
}

internal static class MBType
{
    /// <summary>
    /// メッセージ ボックスには、[ OK] という 1 つのプッシュ ボタンが含まれています。 既定値です。
    /// </summary>
    public const uint Ok = 0x000000000;

    /// <summary>
    /// メッセージ ボックスには、[ OK] と [キャンセル] の 2 つのプッシュ ボタンが含まれています。
    /// </summary>
    public const uint OkCancel = 0x00000001;

    /// <summary>
    /// メッセージ ボックスには、 中止、 再試行、 無視の 3 つのプッシュ ボタンが含まれています。
    /// </summary>
    public const uint AbortRetryIgnore = 0x00000002;

    /// <summary>
    /// メッセージ ボックスには、[ はい ] と [ いいえ] の 2 つのプッシュ ボタンが含まれています。
    /// </summary>
    public const uint YesNoCancel = 0x00000003;

    /// <summary>
    /// メッセージ ボックスには、[ はい]、[ いいえ]、[ キャンセル] の 3 つのプッシュ ボタンが含まれています。
    /// </summary>
    public const uint YesNo = 0x00000004;

    /// <summary>
    /// メッセージ ボックスには、 再試行 と キャンセルの 2 つのプッシュ ボタンが含まれています。
    /// </summary>
    public const uint RetryCancel = 0x00000005;

    /// <summary>
    /// メッセージ ボックスには、 Cancel、 Try Again、 Continue の 3 つのプッシュ ボタンが含まれています。 AbortRetryIgnore の代わりに、このメッセージ ボックスの種類を使用します。
    /// </summary>
    public const uint CancelTryContinue = 0x00000006;

    /// <summary>
    /// メッセージ ボックスに [ヘルプ ] ボタンを追加します。 ユーザーが [ヘルプ ] ボタンをクリックするか F1 キーを押すと、 WM_HELPメッセージが 所有者に送信されます。
    /// </summary>
    public const uint Help = 0x00004000;
}

internal static class MBIcon
{
    /// <summary>
    /// メッセージ ボックスに停止記号アイコンが表示されます。
    /// </summary>
    public const uint Stop = 0x00000010;

    /// <summary>
    /// メッセージ ボックスに停止記号アイコンが表示されます。
    /// </summary>
    public const uint Error = 0x00000010;

    /// <summary>
    /// メッセージ ボックスに疑問符アイコンが表示されます。 疑問符は、質問の特定の種類を明確に表さず、メッセージの言い回しはどのメッセージの種類にも適用されるため、疑問符のメッセージ アイコンは推奨されなくなりました。
    [Obsolete]
    public const uint Question = 0x00000020;

    /// <summary>
    /// 感嘆符アイコンがメッセージ ボックスに表示されます。
    /// </summary>
    public const uint Exclamation = 0x00000030;

    /// <summary>
    /// 感嘆符アイコンがメッセージ ボックスに表示されます。
    /// </summary>
    public const uint Warning = 0x00000030;

    /// <summary>
    /// メッセージ ボックスに、円の中の小文字 i で構成されるアイコンが表示されます。
    /// </summary>
    public const uint Infomation = 0x00000040;

    /// <summary>
    /// メッセージ ボックスに、円の中の小文字 i で構成されるアイコンが表示されます。
    /// </summary>
    public const uint Asterisk = 0x00000040;
}

internal static class MBDefault
{
    /// <summary>
    /// 最初のボタンが既定のボタンです。
    /// Button2、Button3、またはButton4が指定されていない限り、Button1が既定値です。
    /// </summary>
    public const uint Button1 = 0x000000000;

    /// <summary>
    /// 2 番目のボタンが既定のボタンです。
    /// </summary>
    public const uint Button2 = 0x00000100;

    /// <summary>
    /// 3 番目のボタンが既定のボタンです。
    /// </summary>
    public const uint Button3 = 0x00000200;

    /// <summary>
    /// 4 番目のボタンが既定のボタンです。
    /// </summary>
    public const uint Button4 = 0x00000300;
}

internal enum MBResult : int
{
    Ok = 1,
    Cancel = 2,
    Abort = 3,
    Retry = 4,
    Ignore = 5,
    Yes = 6,
    No = 7,
    TryAgain = 10,
    Continue = 11,
}
