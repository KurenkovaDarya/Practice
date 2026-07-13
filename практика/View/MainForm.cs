using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using практика.Model;

namespace практика.View
{
    public class MainForm : Form
    {
        private LibraryManager _library;
        private List<Book> _currentDisplayList;

        private DataGridView dgvBooks;
        private TextBox txtSearch;
        private ComboBox cmbGenreFilter;
        private ComboBox cmbStatusFilter;
        private ComboBox cmbSortBy;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Label lblStatus;

        public MainForm()
        {
            this.Text = "Управление библиотекой";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            _library = new LibraryManager("books.json");
            _currentDisplayList = new List<Book>();

            _library.BooksChanged += Library_BooksChanged;

            CreateControls();
            RefreshBookList();
            LoadGenres();
        }

        private void CreateControls()
        {
            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(10) };

            Label lblSearch = new Label { Text = "Поиск:", Location = new Point(10, 10), Size = new Size(50, 25) };
            txtSearch = new TextBox { Location = new Point(65, 10), Size = new Size(200, 25) };
            txtSearch.TextChanged += (s, e) => ApplyFiltersAndSort();

            Label lblGenre = new Label { Text = "Жанр:", Location = new Point(290, 10), Size = new Size(50, 25) };
            cmbGenreFilter = new ComboBox { Location = new Point(340, 10), Size = new Size(130, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGenreFilter.Items.Add("Все жанры");
            cmbGenreFilter.SelectedIndex = 0;
            cmbGenreFilter.SelectedIndexChanged += (s, e) => ApplyFiltersAndSort();

            Label lblStatusFilter = new Label { Text = "Статус:", Location = new Point(490, 10), Size = new Size(50, 25) };
            cmbStatusFilter = new ComboBox { Location = new Point(545, 10), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatusFilter.Items.Add("Все");
            cmbStatusFilter.Items.Add("Доступна");
            cmbStatusFilter.Items.Add("Занята");
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += (s, e) => ApplyFiltersAndSort();

            Label lblSort = new Label { Text = "Сортировка:", Location = new Point(10, 50), Size = new Size(70, 25) };
            cmbSortBy = new ComboBox { Location = new Point(85, 50), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSortBy.Items.Add("Без сортировки");
            cmbSortBy.Items.Add("Название");
            cmbSortBy.Items.Add("Автор");
            cmbSortBy.Items.Add("Жанр");
            cmbSortBy.SelectedIndex = 0;
            cmbSortBy.SelectedIndexChanged += (s, e) => ApplyFiltersAndSort();

            btnAdd = new Button { Text = "Добавить", Location = new Point(300, 45), Size = new Size(100, 30) };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button { Text = "Редактировать", Location = new Point(410, 45), Size = new Size(100, 30) };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button { Text = "Удалить", Location = new Point(520, 45), Size = new Size(100, 30) };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button { Text = "Обновить", Location = new Point(630, 45), Size = new Size(100, 30) };
            btnRefresh.Click += (s, e) => { RefreshBookList(); LoadGenres(); UpdateStatus("Список обновлён"); };

            panelTop.Controls.AddRange(new Control[] {
                lblSearch, txtSearch,
                lblGenre, cmbGenreFilter,
                lblStatusFilter, cmbStatusFilter,
                lblSort, cmbSortBy,
                btnAdd, btnEdit, btnDelete, btnRefresh
            });

            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
             
            Panel panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 30, Padding = new Padding(10) };
            lblStatus = new Label { Text = "Готово", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            panelBottom.Controls.Add(lblStatus);

            this.Controls.Add(dgvBooks);
            this.Controls.Add(panelBottom);
            this.Controls.Add(panelTop);
        }

        private void Library_BooksChanged(object sender, EventArgs e) => RefreshBookList();

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (BookEditForm editForm = new BookEditForm())
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    _library.AddBook(editForm.Book);
                    UpdateStatus($"Книга \"{editForm.Book.Title}\" добавлена");
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите книгу для редактирования.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedId = (int)dgvBooks.SelectedRows[0].Cells["Id"].Value;
            Book book = _library.Books.FirstOrDefault(b => b.Id == selectedId);
            if (book == null) return;

            using (BookEditForm editForm = new BookEditForm(book))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    _library.EditBook(book.Id, editForm.Book.Title, editForm.Book.Author,
                        editForm.Book.Genre, editForm.Book.IsAvailable);
                    UpdateStatus($"Книга \"{editForm.Book.Title}\" отредактирована");
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите книгу для удаления.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedId = (int)dgvBooks.SelectedRows[0].Cells["Id"].Value;
            Book book = _library.Books.FirstOrDefault(b => b.Id == selectedId);
            if (book == null) return;

            DialogResult result = MessageBox.Show($"Удалить книгу \"{book.Title}\"?", "Подтверждение удаления",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _library.DeleteBook(book.Id);
                UpdateStatus($"Книга \"{book.Title}\" удалена");
            }
        }

        private void RefreshBookList() => ApplyFiltersAndSort();

        private void ApplyFiltersAndSort()
        {
            var query = _library.Books.AsEnumerable();

            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(b => b.Title.ToLower().Contains(searchText.ToLower()) ||
                                          b.Author.ToLower().Contains(searchText.ToLower()));
            }

            if (cmbGenreFilter.SelectedItem != null && cmbGenreFilter.SelectedItem.ToString() != "Все жанры")
            {
                string selectedGenre = cmbGenreFilter.SelectedItem.ToString();
                query = query.Where(b => b.Genre == selectedGenre);
            }

            if (cmbStatusFilter.SelectedItem != null)
            {
                string status = cmbStatusFilter.SelectedItem.ToString();
                if (status == "Доступна")
                    query = query.Where(b => b.IsAvailable);
                else if (status == "Занята")
                    query = query.Where(b => !b.IsAvailable);
            }

            if (cmbSortBy.SelectedItem != null)
            {
                string sortBy = cmbSortBy.SelectedItem.ToString();
                switch (sortBy)
                {
                    case "Название": query = query.OrderBy(b => b.Title); break;
                    case "Автор": query = query.OrderBy(b => b.Author); break;
                    case "Жанр": query = query.OrderBy(b => b.Genre); break;
                }
            }

            _currentDisplayList = query.ToList();
            UpdateDataGridView();
            UpdateStatus($"Показано: {_currentDisplayList.Count} книг");
        }

        private void UpdateDataGridView()
        {
            dgvBooks.DataSource = null;
            dgvBooks.DataSource = _currentDisplayList;
        }

        private void LoadGenres()
        {
            string currentSelection = cmbGenreFilter.SelectedItem?.ToString() ?? "Все жанры";
            cmbGenreFilter.SelectedIndexChanged -= (s, e) => ApplyFiltersAndSort();

            cmbGenreFilter.Items.Clear();
            cmbGenreFilter.Items.Add("Все жанры");

            var genres = _library.GetAllGenres();
            foreach (var genre in genres)
                cmbGenreFilter.Items.Add(genre);

            int index = cmbGenreFilter.Items.IndexOf(currentSelection);
            cmbGenreFilter.SelectedIndex = index >= 0 ? index : 0;
            cmbGenreFilter.SelectedIndexChanged += (s, e) => ApplyFiltersAndSort();
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = $"{DateTime.Now.ToShortTimeString()}: {message}";
        }
    }
}