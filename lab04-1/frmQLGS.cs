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
using lab04_1.Model;
using lab04_1.ViewModel;

namespace lab04_1
{
    
    public partial class frmQLGS : Form
    {
       
        private List<FacultyViewModel> faculties;
        private StudentContextDB db;
        public frmQLGS()
        {
            InitializeComponent();
            db = new StudentContextDB();
            LoadFaculty();
        }
        
        private void LoadFaculty()
        {
            faculties = db.Faculties.Select(k => new FacultyViewModel
            {
                FacultyID = k.FacultyID,
                FacultyName = k.FacultyName,
                TotalProfessor = k.TotalProfessor ?? 0,
            }).ToList();

           
           
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Faculty newFaculty = new Faculty();
                int faculty_ID;
                if (string.IsNullOrWhiteSpace(txtFacultyID.Text) || string.IsNullOrWhiteSpace(txtFacultyName.Text) || string.IsNullOrWhiteSpace(txtTotalPfs.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin khoa.");
                    return;
                }
                if (!int.TryParse(txtFacultyID.Text, out faculty_ID))
                {
                    MessageBox.Show("Mã khoa không hợp lệ. Vui lòng nhập một số hợp lệ.");
                    return;
                }
                newFaculty.FacultyID = faculty_ID;

                newFaculty.FacultyName = txtFacultyName.Text;

                int total_Professor;
                if (!int.TryParse(txtTotalPfs.Text, out total_Professor))
                {
                    MessageBox.Show("Vui lòng nhập một số hợp lệ.");
                    return;
                }
                newFaculty.TotalProfessor = total_Professor;

                var existingFaculty = db.Faculties.FirstOrDefault(f => f.FacultyID == newFaculty.FacultyID);

                if (existingFaculty != null)
                {
                    existingFaculty.FacultyName = newFaculty.FacultyName;
                    existingFaculty.TotalProfessor = newFaculty.TotalProfessor;
                    MessageBox.Show("Đã cập nhật khoa thành công!");
                }
                
                else
                {
                    
                    db.Faculties.Add(newFaculty);
                    MessageBox.Show("Đã thêm khoa mới thành công!");
                }

                db.SaveChanges();
                LoadFaculty();
                BindToGrid(faculties);
                ClearInputFields();
            }
            catch 
            {
                MessageBox.Show("Lỗi!!!!!");
            }
            
        }
        private void BindToGrid(List<FacultyViewModel> faculties)
        {
            dgvFaculty.DataSource = faculties;
        }
        private void frmQLGS_Load(object sender, EventArgs e)
        {
            BindToGrid(faculties);
        }
       

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                int facultyID;
                if (!int.TryParse(txtFacultyID.Text, out facultyID))
                {
                    MessageBox.Show("Mã khoa không hợp lệ.");
                    return;
                }

                var faculty = db.Faculties.FirstOrDefault(k => k.FacultyID == facultyID);
                if (faculty != null)
                {
                    var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa khoa này?", "Xác nhận xóa", MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        db.Faculties.Remove(faculty);
                        db.SaveChanges();
                        LoadFaculty();
                        BindToGrid(faculties);
                        MessageBox.Show("Khoa đã được xóa thành công.");
                    }

                }
                else
                {
                    MessageBox.Show("Không tìm thấy khoa.");
                }
                ClearInputFields();
            }
            catch 
            {
                MessageBox.Show("Lỗi");
            }
           
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvFaculty_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvFaculty.Rows[e.RowIndex];

                txtFacultyID.Text = row.Cells["FacultyID"].Value.ToString();
                txtFacultyName.Text = row.Cells["FacultyName"].Value.ToString();
                txtTotalPfs.Text = row.Cells["TotalProfessor"].Value.ToString();

               
            }
        }
        private void ClearInputFields()
        {
            txtFacultyID.Text = string.Empty;
            txtFacultyName.Text = string.Empty;
            txtTotalPfs.Text = string.Empty;

        }
    }
}
