using System;
using System.Drawing;
using System.Windows.Forms;
using практика.Model;

namespace практика.View
{
    public class BookEditForm : Form
    {
        public Book Book { get; private set; }

        private TextBox txtTitle;
        private TextBox txtAuthor;
        private TextBox txtGenre;
        private ComboBox cmbStatus;
        private Button btnSave;
        private Button btnCancel;

        public BookEditForm(Book book = null)
        {
            this.Size = new Size(400, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            if (book == null)
            {
                Book = new Book();
                this.Text = "Добавление книги";
            }
            else
            {
                Book = new Book(book.Id, book.Title, book.Author, book.Genre, book.IsAvailable);
                this.Text = "Редактирование книги";
            }

            CreateControls();

            if (book != null)
            {
                txtTitle.Text = book.Title;
                txtAuthor.Text = book.Author;
                txtGenre.Text = book.Genre;
                cmbStatus.SelectedIndex = book.IsAvailable ? 0 : 1;
            }
        }

        private void CreateControls()
        {
            int y = 20;

            Label lblTitle = new Label { Text = "Название:", Location = new Point(20, y), Size = new Size(80, 25) };
            txtTitle = new TextBox { Location = new Point(110, y), Size = new Size(240, 25) };
            y += 35;

            Label lblAuthor = new Label { Text = "Автор:", Location = new Point(20, y), Size = new Size(80, 25) };
            txtAuthor = new TextBox { Location = new Point(110, y), Size = new Size(240, 25) };
            y += 35;

            Label lblGenre = new Label { Text = "Жанр:", Location = new Point(20, y), Size = new Size(80, 25) };
            txtGenre = new TextBox { Location = new Point(110, y), Size = new Size(240, 25) };
            y += 35;

            Label lblStatus = new Label { Text = "Статус:", Location = new Point(20, y), Size = new Size(80, 25) };
            cmbStatus = new ComboBox
            {
                Location = new Point(110, y),
                Size = new Size(240, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.Add("Доступна");
            cmbStatus.Items.Add("Занята");
            cmbStatus.SelectedIndex = 0;
            y += 45;

            btnSave = new Button { Text = "Сохранить", Location = new Point(100, y), Size = new Size(80, 30) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button { Text = "Отмена", Location = new Point(200, y), Size = new Size(80, 30) };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] {
                lblTitle, txtTitle,
                lblAuthor, txtAuthor,
                lblGenre, txtGenre,
                lblStatus, cmbStatus,
                btnSave, btnCancel
            });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название книги.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Введите автора книги.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAuthor.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGenre.Text))
            {
                MessageBox.Show("Введите жанр книги.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGenre.Focus();
                return;
            }

            Book.Title = txtTitle.Text.Trim();
            Book.Author = txtAuthor.Text.Trim();
            Book.Genre = txtGenre.Text.Trim();
            Book.IsAvailable = cmbStatus.SelectedIndex == 0;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
