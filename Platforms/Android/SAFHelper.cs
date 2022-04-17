using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using AndroidX.DocumentFile.Provider;

namespace MAUIPickFile
{
    public static class SAFHelper
    {
        public class SAFFileList
        {
            public DocumentFile this[string path] => _getDocumentFileFromPath(path);
        }

        //communication
        internal static bool? _result_1_bool;
        internal static Activity Activity { get => _activity ??= Platform.CurrentActivity; set => _activity = value; }
        internal static int REQUEST_CODE_FOR_DIR = 1;
        internal static int REQUEST_CODE_FOR_MANAGE = 2;

        //public
        public static SAFFileList DocumentFile => _safFileList ??= new SAFFileList();
        /// <summary>
        /// Get DocumentFile of /sdcard/Android/data
        /// </summary>
        public static DocumentFile DocumentFile_AndroidData => _getDocumentFileFromPath(Str_Path_AndroidData);
        /// <summary>
        /// Get DocumentFile of /sdcard/Android/obb
        /// </summary>
        public static DocumentFile DocumentFile_AndroidObb => _getDocumentFileFromPath(Str_Path_AndroidObb);
        

        /// <summary>
        /// Ask for the permission of manage or read and write sdcard files
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RequestFilePermission() => (await _askForWritePermission()) ?? (await _askForManagePermission());

        /// <summary>
        /// Ask for the permission of path
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RequestDocumentFilePermission(string path) => await _askForDocumentFilePermission(path);

        /// <summary>
        /// Ask for the permission of /sdcard/Android/data
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RequestAndroidDataPermission() => await _askForDocumentFilePermission(Str_Path_AndroidData);

        /// <summary>
        /// Ask for the permission of /sdcard/Android/obb
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RequestAndroidObbPermission() => await _askForDocumentFilePermission(Str_Path_AndroidObb);

        public static DocumentFile GetSubDocumentFileFromDocumentFile(DocumentFile documentFile, params string[] subPath)
        {
            DocumentFile ans = documentFile;
            foreach (string str in subPath)
            {
                ans = ans.FindFile(str);
                if (ans == null) return null;
            }
            return ans;
        }

        public static DocumentFile CreateSubDocumentFileFromDocumentFile(DocumentFile documentFile, params string[] subPath)
        {
            DocumentFile ans = documentFile;
            for (int i = 0; i < subPath.Length - 1; i++)
            {
                ans = ans.FindFile(subPath[i]) ?? ans.CreateDirectory(subPath[i]);
            }
            return ans.FindFile(subPath[^1]) ?? ans.CreateFile(Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(Path.GetExtension(subPath[^1])), subPath[^1]);
        }

        /// <summary>
        /// Get file or folder from /sdcard/Android/data. If the file or folder is not exist, return null.
        /// </summary>
        /// <param name="subPath"></param>
        /// <returns></returns>
        public static DocumentFile GetSubDocumentFileFromAndroidData(params string[] subPath) => GetSubDocumentFileFromDocumentFile(DocumentFile_AndroidData, subPath);

        /// <summary>
        /// Get file or folder from /sdcard/Android/obb. If the file or folder is not exist, return null.
        /// </summary>
        /// <param name="subPath"></param>
        /// <returns></returns>
        public static DocumentFile GetSubDocumentFileFromAndroidObb(params string[] subPath) => GetSubDocumentFileFromDocumentFile(DocumentFile_AndroidObb, subPath);

        /// <summary>
        /// Create file and parent folder in /sdcard/Android/data.
        /// </summary>
        /// <param name="subPath"></param>
        /// <returns></returns>
        public static DocumentFile CreateSubDocumentFileFromAndroidData(params string[] subPath) => CreateSubDocumentFileFromDocumentFile(DocumentFile_AndroidData, subPath);

        /// <summary>
        /// Create file and parent folder in /sdcard/Android/obb.
        /// </summary>
        /// <param name="subPath"></param>
        /// <returns></returns>
        public static DocumentFile CreateSubDocumentFileFromAndroidObb(params string[] subPath) => CreateSubDocumentFileFromDocumentFile(DocumentFile_AndroidObb, subPath);

        /// <summary>
        /// Get all files (excluding files in all sub folders) and folders from DocumentFile. If the DocumentFile is not a folder, return null.
        /// </summary>
        /// <param name="documentFile"></param>
        /// <returns></returns>
        public static DocumentFile[] GetSubFileAndFolder(this DocumentFile documentFile) => _getSubFileAndFolder(documentFile);

        /// <summary>
        /// Get all files (including files in each sub folder) from DocumentFile. If the DocumentFile is not a folder, return null.
        /// </summary>
        /// <param name="documentFile"></param>
        /// <returns></returns>
        public static DocumentFile[] GetSubFiles(this DocumentFile documentFile) => _getSubFiles(documentFile);
        //private
        //Activity
        static Activity _activity;
        static SAFFileList _safFileList;
        static readonly string Str_Path_AndroidData = "/sdcard/Android/data";
        static readonly string Str_Path_AndroidObb = "/sdcard/Android/obb";

        //Ask for file manage permission
        static async Task<bool> _askForManagePermission()
        {
#pragma warning disable CA1416
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R && !Android.OS.Environment.IsExternalStorageManager)
            {
                var bb = new Intent(Settings.ActionManageAllFilesAccessPermission);
                _result_1_bool = null;
                Activity.StartActivityForResult(bb, REQUEST_CODE_FOR_MANAGE);
                while (_result_1_bool == null) await Task.Delay(300);
                return _result_1_bool ?? false;
            }
            return false;
#pragma warning restore CA1416
        }

        //Ask for file write permission
        static async Task<bool?> _askForWritePermission()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.R)
            {
                ReadWriteStoragePermission readwritepermission = new ReadWriteStoragePermission();
                PermissionStatus status = await readwritepermission.CheckStatusAsync();
                if (status != PermissionStatus.Granted) return (await readwritepermission.RequestAsync()) == PermissionStatus.Granted;
                return true;
            }
            return null;
        }

        class ReadWriteStoragePermission : Permissions.BasePlatformPermission
        {
            public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new (string androidPermission, bool isRuntime)[2]
            {
                (Android.Manifest.Permission.ReadExternalStorage, true),
                (Android.Manifest.Permission.WriteExternalStorage, true)
            };
        }

        //Ask
        static async Task<bool> _askForDocumentFilePermission(string path)
        {
            string uri = _changeToUri_AndroidData(path);
            Android.Net.Uri parse = Android.Net.Uri.Parse(uri);
            Intent intent = new Intent(Intent.ActionOpenDocumentTree);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantPersistableUriPermission | ActivityFlags.GrantPrefixUriPermission);
            intent.PutExtra(DocumentsContract.ExtraInitialUri, parse);
            _result_1_bool = null;
            Activity.StartActivityForResult(intent, REQUEST_CODE_FOR_DIR);
            while (_result_1_bool == null) await Task.Delay(300);
            return _result_1_bool ?? false;
        }

        //Get
        static DocumentFile _getDocumentFileFromPath(string path) => AndroidX.DocumentFile.Provider.DocumentFile.FromTreeUri(Activity, Android.Net.Uri.Parse(_changeToUri_DocumentsTree(path)));

        static readonly string Str_ChangeToUri_PathSeparator_1 = "/";
        static readonly string Str_ChangeToUri_PathSeparator_2 = "%2F";
        static readonly string Str_ChangeToUri_Sdcard_1 = "/sdcard/";
        static readonly string Str_ChangeToUri_Sdcard_2 = "/storage/emulated/0/";
        static readonly string Str_ChangeToUri_DocumentsTree = "content://com.android.externalstorage.documents/tree/primary%3A";
        static readonly string Str_ChangeToUri_AndroidData = "content://com.android.externalstorage.documents/tree/primary%3AAndroid%2Fdata/document/primary%3A";

        static DocumentFile[] _getSubFileAndFolder(DocumentFile documentFile)
        {
            if (documentFile.IsDirectory)
            {
                List<DocumentFile> ans = new List<DocumentFile>();
                foreach (DocumentFile subDocumentFile in documentFile.ListFiles())
                {
                    ans.Add(subDocumentFile);
                }
                return ans.ToArray();
            }
            return null;
        }

        static DocumentFile[] _getSubFiles(DocumentFile documentFile)
        {
            if (!documentFile.IsDirectory) return null;
            List<DocumentFile> ans = new List<DocumentFile>();
            void Add(DocumentFile d)
            {
                if (d.IsDirectory)
                {
                    foreach (DocumentFile subDocumentFile in d.ListFiles())
                    {
                        Add(subDocumentFile);
                    }
                }
                else
                {
                    ans.Add(d);
                }
            }
            Add(documentFile);
            return ans.ToArray();
        }

        static string _changeToUri_DocumentsTree(string path) => Str_ChangeToUri_DocumentsTree + _formatSdcardPath(path).Replace(Str_ChangeToUri_PathSeparator_1, Str_ChangeToUri_PathSeparator_2);

        static string _changeToUri_AndroidData(string path) => Str_ChangeToUri_AndroidData + _formatSdcardPath(path).Replace(Str_ChangeToUri_PathSeparator_1, Str_ChangeToUri_PathSeparator_2);

        static string _formatSdcardPath(string path)
        {
            if (path.StartsWith(Str_ChangeToUri_Sdcard_1))
            {
                path = path[Str_ChangeToUri_Sdcard_1.Length..];
            }
            else if (path.StartsWith(Str_ChangeToUri_Sdcard_2))
            {
                path = path[Str_ChangeToUri_Sdcard_2.Length..];
            }
            else if (path.StartsWith(Str_ChangeToUri_PathSeparator_1))
            {
                path = path[Str_ChangeToUri_PathSeparator_1.Length..];
            }
            if (path.EndsWith(Str_ChangeToUri_PathSeparator_1))
            {
                path = path[..^Str_ChangeToUri_PathSeparator_1.Length];
            }
            return path;
        }
    }
}
