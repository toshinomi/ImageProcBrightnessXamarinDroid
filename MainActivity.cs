using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Graphics;
using System;
using System.Threading.Tasks;

namespace ImageProcBrightnessXamarinDroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Bitmap      mBitmap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            InitLayout();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void InitLayout()
        {
            mBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.dog);

            var btnReset = (Button)FindViewById(Resource.Id.reset);
            btnReset.Click += OnClickBtnReset;

            var seekBarAlpha = (SeekBar)FindViewById(Resource.Id.alpha_seekBar);
            seekBarAlpha.ProgressChanged += new EventHandler<SeekBar.ProgressChangedEventArgs>(OnProgressChanged);
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
    }
}