using HotelManager.DAO;
using HotelManager.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager
{
    public partial class fProfile : Form
    {
        public fProfile(string userName)
        {
            InitializeComponent();
            LoadProfile(userName);
        }
        string Password;
        public void LoadProfile(string username)
        {
            Account staff = AccountDAO.Instance.LoadStaffInforByUserName(username);
            lblUserName.Text = txbUserName.Text = staff.UserName;
            lblDisplayName.Text = txbDisplayName.Text = staff.DisplayName;
            txbStaffType.Text = AccountTypeDAO.Instance.GetStaffTypeByUserName(username).Name;
            txbIDCard.Text = staff.IdCard.ToString();
            txbPhoneNumber.Text = staff.PhoneNumber.ToString();
            txbAddress.Text = staff.Address;
            dpkDateOfBirth.Value = staff.DateOfBirth;
            cbSex.Text = staff.Sex;
            txbStartDay.Text = staff.StartDay.ToShortDateString();
            Password = staff.PassWord;
        }
        public void UpdateDisplayName(string username, string displayname)
        {
            AccountDAO.Instance.UpdateDisplayName(username, displayname);
        }
        public void UpdatePassword(string username, string password)
        {
            AccountDAO.Instance.UpdatePassword(username, password);
        }
        public void UpdateInfo(string username, string address, int phonenumber,string idCard, DateTime dateOfBirth, string sex)
        {
            AccountDAO.Instance.UpdateInfo(username, address, phonenumber,idCard,dateOfBirth,sex);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBookRoom_Click(object sender, EventArgs e)
        {
            if(txbDisplayName.Text!=String.Empty)

            {
                UpdateDisplayName(txbUserName.Text, txbDisplayName.Text);
                MessageBox.Show( "Cập nhật thông tin tài khoản thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadProfile(txbUserName.Text);
            }
            else
                MessageBox.Show( "Tên hiển thị không hợp lệ.\nVui lòng nhập lại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void bunifuThinButton22_Click(object sender, EventArgs e)
        {
            if (AccountDAO.Instance.HashPass(txbPass.Text) ==Password && txbNewPass.Text!=String.Empty && txbReNewPass.Text!=String.Empty)
            {
                if(txbNewPass.Text==txbReNewPass.Text)
                {
                    UpdatePassword(txbUserName.Text, txbNewPass.Text);
                    MessageBox.Show( "Cập nhật mật khẩu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txbPass.Text = txbNewPass.Text = txbReNewPass.Text = String.Empty;
                    LoadProfile(txbUserName.Text);
                }
                else
                {
                    MessageBox.Show( "Mật khẩu mới không hợp lệ.\nVui lòng nhập lại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txbNewPass.Text = txbReNewPass.Text = String.Empty;
                }
            }
            else
            {
                MessageBox.Show( "Mật khẩu không hợp lệ.\nVui lòng nhập lại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbPass.Text=txbNewPass.Text = txbReNewPass.Text = String.Empty;
            }   
        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbAddress.Text) ||
                string.IsNullOrWhiteSpace(txbPhoneNumber.Text) ||
                string.IsNullOrWhiteSpace(cbSex.Text) ||
                dpkDateOfBirth.Value >= DateTime.Now.Date)
            {
                MessageBox.Show("Thông tin cơ bản không hợp lệ.\nVui lòng nhập lại.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra số điện thoại hợp lệ
            if (!int.TryParse(txbPhoneNumber.Text, out int phoneNumber))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra CMND/CCCD hợp lệ
            if (!Regex.IsMatch(txbIDCard.Text, @"^\d{9}$|^\d{12}$"))
            {
                MessageBox.Show("Số CMND/CCCD không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra giới tính hợp lệ
            if (cbSex.Text != "Nam" && cbSex.Text != "Nữ")
            {
                MessageBox.Show("Giới tính không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra CMND/CCCD có tồn tại hay không
            if (!AccountDAO.Instance.IsIdCardExists(txbIDCard.Text))
            {
                MessageBox.Show("Thẻ căn cước/CMND không tồn tại.\nVui lòng nhập lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Cập nhật thông tin
            UpdateInfo(txbUserName.Text, txbAddress.Text, phoneNumber, txbIDCard.Text, dpkDateOfBirth.Value, cbSex.Text);
            MessageBox.Show("Cập nhật thông tin cơ bản thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Load lại hồ sơ
            LoadProfile(txbUserName.Text);
        }


        private void txbPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void btnClose__Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txbIDCard_OnValueChanged(object sender, EventArgs e)
        {

        }
    }
}
