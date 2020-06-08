using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Graphics;
using System;
using System.Threading.Tasks;
using Android.Content;

namespace ImageProcBrightnessXamarinDroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Bitmap mBitmap;
        private const int READ_REQUEST_CODE = 42;
        private const string ERROR_MESSAGE_SELECT_IMAGE = "画像選択のエラーが発生しました";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            InitLayout();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Ok)
            {
                return;
            }

            switch (requestCode)
            {
                case READ_REQUEST_CODE:
                    try
                    {
                        if (data?.Data != null)
                        {
                            var uri = data?.Data;
                            var inputStream = ContentResolver?.OpenInputStream(uri);
                            var image = BitmapFactory.DecodeStream(inputStream);
                            var imageView = (ImageView)FindViewById(Resource.Id.image);
                            imageView.SetImageBitmap(image);
                            mBitmap = image;
                        }
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(this, ERROR_MESSAGE_SELECT_IMAGE, ToastLength.Long).Show();
                    }
                    break;
                default:
                    break;
            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// レイアウトの初期設定
        /// </summary>
        private void InitLayout()
        {
            mBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.dog);

            var btnReset = (Button)FindViewById(Resource.Id.reset);
            btnReset.Click += OnClickBtnReset;

            var seekBarAlpha = (SeekBar)FindViewById(Resource.Id.alpha_seekBar);
            seekBarAlpha.ProgressChanged += new EventHandler<SeekBar.ProgressChangedEventArgs>(OnProgressChanged);

            var btnImageSelect = (Button)FindViewById(Resource.Id.select_image);
            btnImageSelect.Click += OnClickBtnSelectImage;
        }

        /// <summary>
        /// リセットボタンのクリックイベント
        /// </summary>
        /// <param name="s">オブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void OnClickBtnReset(object s, EventArgs e)
        {
            var imageView = (ImageView)FindViewById(Resource.Id.image);
            imageView?.SetImageBitmap(mBitmap);
        }

        /// <summary>
        /// シークバーのプログレスチェンジイベント
        /// </summary>
        /// <param name="s">オブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private async void OnProgressChanged(object s, SeekBar.ProgressChangedEventArgs e)
        {
            var textViewAlphaValue = (TextView)FindViewById(Resource.Id.alpha_value);
            textViewAlphaValue.Text = e.Progress.ToString() + " %";
            var brightness = new Brightness();
            var mutableBitmap = await Task.Run(() => brightness.goImageProcessing(mBitmap, e.Progress));
            var imageView = (ImageView)FindViewById(Resource.Id.image);
            imageView.SetImageBitmap(mutableBitmap.Copy(Bitmap.Config.Argb8888, false));
        }

        /// <summary>
        /// 画像選択のクリックイベント
        /// </summary>
        /// <param name="s">オブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void OnClickBtnSelectImage(object s, EventArgs e)
        {
            using (var intent = new Intent(Intent.ActionOpenDocument))
            {
                intent.AddCategory(Intent.CategoryOpenable);
                intent.SetType("image/*");
                StartActivityForResult(intent, READ_REQUEST_CODE);
            }
            
        }
    }
}