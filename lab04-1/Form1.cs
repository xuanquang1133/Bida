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
using lab04_1.Model;
using lab04_1.ViewModel;

namespace lab04_1
{
  
    public partial class Form1 : Form
    {
        public List<FacultyViewModel> faculties;
        public List<StudentViewModel> students;
        public StudentContextDB db;
        public Form1()
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

        private void Form1_Load(object sender, EventArgs e)
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
            dgvStudent.DataSource = students;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                
                
                Student newStudent = new Student();
                newStudent.StudentID = txtMSSV.Text;
                newStudent.StudentName = txtName.Text;
                newStudent.FacultyID = (cmbFaculty.SelectedItem as FacultyViewModel).FacultyID;
                double averageScore;
                if (string.IsNullOrWhiteSpace(txtMSSV.Text) || string.IsNullOrWhiteSpace(txtName.Text) || cmbFaculty.SelectedItem == null || string.IsNullOrWhiteSpace(txtAVG.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên.");
                    return;
                }
                if (!double.TryParse(txtAVG.Text, out averageScore))
                {
                    MessageBox.Show("Điểm trung bình không hợp lệ. Vui lòng nhập một số hợp lệ.");
                    return;
                }
                newStudent.AverageScore = averageScore;

                var existingStudent = db.Students.FirstOrDefault(s => s.StudentID == txtMSSV.Text);
                if (existingStudent != null)
                {
                    MessageBox.Show("Mã số sinh viên đã tồn tại. Vui lòng nhập mã khác.");
                    return;
                }


                db.Students.Add(newStudent);
                MessageBox.Show("Đã thêm sinh viên thành công!");
                db.SaveChanges();

                LoadStudent();
                BindToGrid(students);
                ClearInputFields();
            }
            catch 
            {
                MessageBox.Show("Lỗi!!");
            }   
            

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var studentToUpdate = db.Students.FirstOrDefault(s => s.StudentID == txtMSSV.Text);
            if (studentToUpdate != null)
            {
                studentToUpdate.StudentName = txtName.Text;
                studentToUpdate.FacultyID = (cmbFaculty.SelectedItem as FacultyViewModel).FacultyID;

                double averageScore;
                if (!double.TryParse(txtAVG.Text, out averageScore))
                {
                    MessageBox.Show("Điểm trung bình không hợp lệ. Vui lòng nhập một số hợp lệ.");
                    return;
                }
                studentToUpdate.AverageScore = averageScore;
                MessageBox.Show("Đã cập nhật sinh viên thành công!");
                db.SaveChanges();

                LoadStudent();
                BindToGrid(students);
                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên cần sửa đổi.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var studentToDelete = db.Students.FirstOrDefault(s => s.StudentID == txtMSSV.Text);
            if (studentToDelete != null)
            {
                db.Students.Remove(studentToDelete);
                db.SaveChanges();

                LoadStudent();
                BindToGrid(students);
                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên cần xóa.");
            }
        }
        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStudent.Rows[e.RowIndex];

                txtMSSV.Text = row.Cells["StudentID"].Value.ToString();
                txtName.Text = row.Cells["StudentName"].Value.ToString();
                txtAVG.Text = row.Cells["AverageScore"].Value.ToString();

                var facultyName = row.Cells["FacultyName"].Value.ToString();
                cmbFaculty.SelectedIndex = cmbFaculty.FindStringExact(facultyName);
            }
        }

        private void ClearInputFields()
        {
            txtMSSV.Clear();   
            txtName.Clear();   
            txtAVG.Clear();     
            cmbFaculty.SelectedIndex = -1;   
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnQLGS_Click(object sender, EventArgs e)
        {
            frmQLGS formQLGS = new frmQLGS();
            this.Hide();
            formQLGS.ShowDialog();
            this.Show();
        }

        private void tlsSearch_Click(object sender, EventArgs e)
        {
            StudentSearch studentSearch = new StudentSearch();
            this.Hide();
            studentSearch.ShowDialog();
            this.Show();
        }
    }
}
