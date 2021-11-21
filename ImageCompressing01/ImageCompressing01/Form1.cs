using ImageCompressing01.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageCompressing01
{
    public partial class Form1 : Form
    {
        PictureBox firstImageBox;
        PictureBox secondImageBox;
        Label mseR;
        Label mseG;
        Label mseB;
        Label mseFull;
        Label psnrR;
        Label psnrG;
        Label psnrB;
        Label psnrFull;
        Label psnrException;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var loadFirstImageButton = new Button();
            loadFirstImageButton.Size = new Size(100,20);
            loadFirstImageButton.Location = new Point(256, 600);
            loadFirstImageButton.Text = "Загрузить изображение";
            loadFirstImageButton.Click += LoadFirstImageButton_Click;
            this.Controls.Add(loadFirstImageButton);

            var loadSecondImageButton = new Button();
            loadSecondImageButton.Size = new Size(100,20);
            loadSecondImageButton.Location = new Point(918, 600);
            loadSecondImageButton.Text = "Загрузить изображение";
            loadSecondImageButton.Click += LoadSecondImageButton_Click;
            this.Controls.Add(loadSecondImageButton);

            var saveFirstImageButton = new Button();
            saveFirstImageButton.Size = new Size(100, 20);
            saveFirstImageButton.Location = new Point(256, 630);
            saveFirstImageButton.Text = "Сохранить изображение";
            saveFirstImageButton.Click += SaveFirstImageButton_Click;
            this.Controls.Add(saveFirstImageButton);

            var saveSecondImageButton = new Button();
            saveSecondImageButton.Size = new Size(100, 20);
            saveSecondImageButton.Location = new Point(918, 630);
            saveSecondImageButton.Text = "Сохранить изображение";
            saveSecondImageButton.Click += SaveSecondImageButton_Click;
            this.Controls.Add(saveSecondImageButton);

            firstImageBox = new PictureBox();
            firstImageBox.Size = new Size(512, 512);
            firstImageBox.Location = new Point(50, 50);
            this.Controls.Add(firstImageBox);

            secondImageBox = new PictureBox();
            secondImageBox.Size = new Size(512, 512);
            secondImageBox.Location = new Point(680, 50);
            this.Controls.Add(secondImageBox);

            var countPSNRButton = new Button();
            countPSNRButton.Size = new Size(100, 20);
            countPSNRButton.Location = new Point(580, 610);
            countPSNRButton.Text = "PSNR";
            countPSNRButton.Click += CountPSNR_Click;
            this.Controls.Add(countPSNRButton);

            psnrException = new Label();
            psnrException.Size = new Size(200, 40);
            psnrException.Location = new Point(690, 590);
            psnrException.Text = "Ошибка. Изображения имеют разные размеры.";
            psnrException.ForeColor = Color.Red;
            psnrException.Visible = false;
            this.Controls.Add(psnrException);

            var grayEqualWeightsButton = new Button();
            grayEqualWeightsButton.Size = new Size(100, 20);
            grayEqualWeightsButton.Location = new Point(580, 590);
            grayEqualWeightsButton.Text = "Gray(EW)";
            grayEqualWeightsButton.Click += GrayEqualWeight_Click;
            this.Controls.Add(grayEqualWeightsButton);

            var grayCCIRButton = new Button();
            grayCCIRButton.Size = new Size(100, 20);
            grayCCIRButton.Location = new Point(580, 570);
            grayCCIRButton.Text = "Gray(CCIR)";
            grayCCIRButton.Click += GrayCCIR_Click;
            this.Controls.Add(grayCCIRButton);

            var toYCbCrButton = new Button();
            toYCbCrButton.Size = new Size(100, 20);
            toYCbCrButton.Location = new Point(580, 550);
            toYCbCrButton.Text = "to Y Cb Cr";
            toYCbCrButton.Click += YCbCr_Click;
            this.Controls.Add(toYCbCrButton);

            var toRGBButton = new Button();
            toRGBButton.Size = new Size(100, 20);
            toRGBButton.Location = new Point(580, 530);
            toRGBButton.Text = "to R G B";
            toRGBButton.Click += RGB_Click;
            this.Controls.Add(toRGBButton);

            this.Controls.Add(toYCbCrButton);
            var table = new TableLayoutPanel();
            table.RowCount = 5;
            table.ColumnCount = 3;
            table.Size = new Size(320, 125);
            table.Location = new Point(473, 635);
            table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            var mseTitle = new Label(){ Text = "MSE" };
            var psnrTitle = new Label(){ Text = "PSNR" };
            var RTitle = new Label(){ Text = "R" };
            var GTitle = new Label(){ Text = "G" };
            var BTitle = new Label(){ Text = "B" };
            var FullTitle = new Label(){ Text = "Full"};

            table.Controls.Add(mseTitle, 1, 0);
            table.Controls.Add(psnrTitle, 2, 0);
            table.Controls.Add(RTitle, 0, 1);
            table.Controls.Add(GTitle, 0, 2);
            table.Controls.Add(BTitle, 0, 3);
            table.Controls.Add(FullTitle, 0, 4);

            mseR = new Label() { Text = "" };
            mseG = new Label() { Text = "" };
            mseB = new Label() { Text = "" };
            mseFull = new Label() { Text = "" };
            psnrR = new Label() { Text = "" };
            psnrG = new Label() { Text = "" };
            psnrB = new Label() { Text = "" };
            psnrFull = new Label() { Text = "" };

            table.Controls.Add(mseR, 1, 1);
            table.Controls.Add(mseG, 1, 2);
            table.Controls.Add(mseB, 1, 3);
            table.Controls.Add(mseFull, 1, 4);
            table.Controls.Add(psnrR, 2, 1);
            table.Controls.Add(psnrG, 2, 2);
            table.Controls.Add(psnrB, 2, 3);
            table.Controls.Add(psnrFull, 2, 4);
            this.Controls.Add(table);

            var timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += (object s, EventArgs ev) => { psnrException.Visible = false; };
            timer.Start();
        }

        private void GrayEqualWeight_Click(object sender, EventArgs e)
        {
            var bitmap = ImageTransformer.ToGrayByEqualWeights(firstImageBox.Image);

            secondImageBox.Image = bitmap;
        }

        private void GrayCCIR_Click(object sender, EventArgs e)
        {
            var bitmap = ImageTransformer.ToGrayByCCIR(firstImageBox.Image);

            secondImageBox.Image = bitmap;
        }

        private void YCbCr_Click(object sender, EventArgs e)
        {
            secondImageBox.Image = ImageTransformer.ToYCbCr(firstImageBox.Image);
        }

        private void RGB_Click(object sender, EventArgs e)
        {
            secondImageBox.Image = ImageTransformer.ToRGB(secondImageBox.Image);
        }

        private void CountPSNR_Click(object sender, EventArgs e)
        {
            PSNR psnr = null;
            try
            {
                psnr = ImageTransformer.CountPSNR(firstImageBox.Image, secondImageBox.Image);
            }
            catch(Exception exc)
            {
                psnrException.Visible = true;
                return;
            }

            //R
            mseR.Text = psnr.MSE.R.ToString();
            psnrR.Text = psnr.R;
            
            //G
            mseG.Text = psnr.MSE.G.ToString();
            psnrG.Text = psnr.G;

            //B
            mseB.Text = psnr.MSE.B.ToString();
            psnrB.Text = psnr.B;

            //Full
            mseFull.Text = psnr.MSEFull.ToString("0.###");
            psnrFull.Text = psnr.Full;
        }

        private void LoadFirstImageButton_Click(object sender, EventArgs e)
        {
            LoadImage(firstImageBox);
        }

        private void LoadSecondImageButton_Click(object sender, EventArgs e)
        {
            LoadImage(secondImageBox);
        }

        private void SaveFirstImageButton_Click(object sender, EventArgs e)
        {
            SaveImage(firstImageBox);
        }

        private void SaveSecondImageButton_Click(object sender, EventArgs e)
        {
            SaveImage(secondImageBox);
        }

        public void LoadImage(PictureBox pictureBox)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Файлы изображений|*.bmp;*.png;*.jpg";
            if (openDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                var bitmap = LoadBitmap(openDialog.FileName);
                pictureBox.Image = bitmap;
            }
            catch (OutOfMemoryException ex)
            {
                MessageBox.Show("Ошибка чтения картинки");
                return;
            }
        }

        public void SaveImage(PictureBox pictureBox)
        {
            if (pictureBox.Image != null)
            {
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                //отображается ли кнопка "Справка" в диалоговом окне
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {
                        pictureBox.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public static Bitmap LoadBitmap(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return new Bitmap(fs);
        }
    }
}
