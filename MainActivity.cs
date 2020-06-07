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
        private ImageView   mImageView;
        private Bitmap      mBitmap;
        private Button      mBtnReset;
        private SeekBar     mSeekBarAlpha;
        private TextView    mTextViewAlphaValue;

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
            mImageView = (ImageView)FindViewById(Resource.Id.image);
            mBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.dog);

            mBtnReset = (Button)FindViewById(Resource.Id.reset);
            mBtnReset.Click += OnClickBtnReset;

            mSeekBarAlpha = (SeekBar)FindViewById(Resource.Id.alpha_seekBar);
            mSeekBarAlpha.ProgressChanged += new EventHandler<SeekBar.ProgressChangedEventArgs>(OnProgressChanged);

            mTextViewAlphaValue = (TextView)FindViewById(Resource.Id.alpha_value);
        }

        /// <summary>
        /// リセットボタンのクリックイベント
        /// </summary>
        /// <param name="s">オブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private void OnClickBtnReset(object s, EventArgs e)
        {
            mImageView.SetImageBitmap(mBitmap);
        }

        /// <summary>
        /// シークバーのプログレスチェンジイベント
        /// </summary>
        /// <param name="s">オブジェクト</param>
        /// <param name="e">イベントのデータ</param>
        private async void OnProgressChanged(object s, SeekBar.ProgressChangedEventArgs e)
        {
            mTextViewAlphaValue.Text = e.Progress.ToString() + " %";
            var brightness = new Brightness();
            var mutableBitmap = await Task.Run(() => brightness.goImageProcessing(mBitmap, e.Progress));
            mImageView.SetImageBitmap(mutableBitmap.Copy(Bitmap.Config.Argb8888, false));
        }
    }
}