using lab04_1.Model;
using lab04_1.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace lab04_1
{
    public partial class StudentSearch : Form
    {
        private StudentContextDB db;
        List<StudentViewModel> students;
        List<FacultyViewModel> faculties;
        public StudentSearch()
        {
            InitializeComponent();
            db = new StudentContextDB();
            LoadStudent();
            LoadFacutiesIntoCmbFaculty();
        }

        private void LoadFacutiesIntoCmbFaculty()
        {
            faculties = db.Faculties.Select(k => new FacultyViewModel
            {
                FacultyID = k.FacultyID,
                FacultyName = k.FacultyName,
            }).ToList();
        }

        private void LoadStudent()
        {
            students = db.Students.Select(sv => new StudentViewModel
            {
                StudentID = sv.StudentID,
                StudentName = sv.StudentName,
                FacultyName = sv.Faculty.FacultyName,
                AverageScore = sv.AverageScore.ToString(),
            }).ToList();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StudentSearch_Load(object sender, EventArgs e)
        {
            BindToGrid(students);
            BindToCombobox(faculties);
            cmbFaculty.SelectedIndex = -1;
        }

        private void BindToCombobox(List<FacultyViewModel> faculties)
        {
            cmbFaculty.DataSource = faculties;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }

        private void BindToGrid(List<StudentViewModel> students)
        {
            dgvSearch.DataSource = students;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchStudents();
        }

        private void SearchStudents()
        {
            string id = txtSearchID.Text.Trim();
            string name = txtSearchName.Text.Trim();
            int? selectedFacultyID = cmbFaculty.SelectedValue as int?;
            var query = db.Students.AsQueryable();
            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(s => s.StudentID.Contains(id));
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.StudentName.Contains(name));
            }

            if (selectedFacultyID.HasValue)
            {
                query = query.Where(s => s.FacultyID == selectedFacultyID);
            }

            List<StudentViewModel> searchResults = query.Select(sv => new StudentViewModel
            {
                StudentID = sv.StudentID,
                StudentName = sv.StudentName,
                FacultyName = sv.Faculty.FacultyName,
                AverageScore = sv.AverageScore.ToString() ?? ""


            }).ToList();
            BindToGrid(searchResults);
            txtResult.Text = "" + searchResults.Count;
        }

        private void dgvSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSearch.SelectedRows.Count > 0)
            {

                DataGridViewRow selectedRow = dgvSearch.SelectedRows[0];

                txtSearchID.Text = selectedRow.Cells[0].Value?.ToString() ?? string.Empty;
                txtSearchName.Text = selectedRow.Cells[1].Value?.ToString() ?? string.Empty;



                string facultyName = selectedRow.Cells[3].Value?.ToString();
                if (facultyName != null)
                {
                    cmbFaculty.SelectedItem = cmbFaculty.Items.Cast<FacultyViewModel>().FirstOrDefault(f => f.FacultyName == facultyName);
                }
            }
        }

        private void btnSearchRemove_Click(object sender, EventArgs e)
        {
            var studentToDelete = db.Students.FirstOrDefault(s => s.StudentID == txtSearchID.Text);
            if (studentToDelete != null)
            {
                db.Students.Remove(studentToDelete);
                db.SaveChanges();
                LoadStudent();
                MessageBox.Show("Xóa thành công");

                BindToGrid(students);
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên cần xóa.");
            }
        }
    }
}
