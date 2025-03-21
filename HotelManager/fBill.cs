using HotelManager.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager
{
    public partial class fBill : Form
    {
        #region Constructor & Properties
        private readonly fPrintBill fPrintBill = new fPrintBill();

        public fBill()
        {
            InitializeComponent();
            dataGridViewBill.Font = new Font("Segoe UI", 9.75F);
            LoadFullBill(GetFullBill());
            comboboxID.DisplayMember = "ID";
            cbBillSearch.SelectedIndex = 0;
        }

        #endregion

        #region Load
        private void LoadFullBill(DataTable table)
        {
            BindingSource source = new BindingSource();
            ChangePrice(table); // Cập nhật giá tiền nếu có logic thay đổi giá

            source.DataSource = table;
            dataGridViewBill.DataSource = source;
            bindingBill.BindingSource = source;

            // Xóa Binding cũ để tránh lỗi hiển thị
            comboboxID.DataBindings.Clear();

            // Kiểm tra nếu có dữ liệu trong bảng
            if (table.Rows.Count > 0)
            {
                comboboxID.DisplayMember = "BillID";  // Hiển thị BillID trên combobox
                comboboxID.ValueMember = "BillID";    // Giá trị chọn cũng là BillID
                comboboxID.DataSource = source;
                comboboxID.SelectedIndex = 0; // Chọn item đầu tiên sau khi load dữ liệu
            }
            else
            {
                comboboxID.DataSource = null; // Nếu không có dữ liệu, tránh lỗi hiển thị
            }

            // Xóa tất cả Binding trước khi thêm mới
            txbDateCreate.DataBindings.Clear();
            txbName.DataBindings.Clear();
            txbPrice.DataBindings.Clear();
            txbStatusRoom.DataBindings.Clear();
            txbUser.DataBindings.Clear();
            txbDiscount.DataBindings.Clear();
            txbFinalPrice.DataBindings.Clear();

            // Thêm Binding mới cho các TextBox
            txbDateCreate.DataBindings.Add("Text", source, "DateOfCreate");
            txbName.DataBindings.Add("Text", source, "roomName");
            txbPrice.DataBindings.Add("Text", source, "totalPrice");
            txbStatusRoom.DataBindings.Add("Text", source, "StatusBill");
            txbUser.DataBindings.Add("Text", source, "StaffSetUp");
            txbDiscount.DataBindings.Add("Text", source, "discount");
            txbFinalPrice.DataBindings.Add("Text", source, "finalprice");

            if (table.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy hóa đơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        #endregion

        #region Change Text
        private void ChangePrice(DataTable table)
        {
            table.Columns.Add("totalPrice_New", typeof(string));
            table.Columns.Add("finalprice_New", typeof(string));
            for (int i = 0; i < table.Rows.Count; i++)
            {
                table.Rows[i]["finalprice_New"] = ((int)table.Rows[i]["finalprice"]).ToString("C0", CultureInfo.CreateSpecificCulture("vi-VN"));
                table.Rows[i]["totalPrice_New"] = ((int)table.Rows[i]["totalPrice"]).ToString("C0", CultureInfo.CreateSpecificCulture("vi-VN"));
            }
            table.Columns.Remove("finalprice");
            table.Columns.Remove("totalPrice");
            table.Columns["totalPrice_New"].ColumnName = "totalPrice";
            table.Columns["finalprice_New"].ColumnName = "finalprice";

        }
        private void BtnSeenBill_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu combobox rỗng
            if (string.IsNullOrWhiteSpace(comboboxID.Text))
            {
                MessageBox.Show("Vui lòng chọn hoặc nhập mã hóa đơn hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra xem comboboxID có phải số hợp lệ không
            if (!int.TryParse(comboboxID.Text, out int billID))
            {
                MessageBox.Show("Mã hóa đơn không hợp lệ! Vui lòng nhập số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Kiểm tra trạng thái phòng trước khi hiển thị hóa đơn
                if (!txbStatusRoom.Text.Contains("Ch"))
                {
                    fPrintBill.SetPrintBill(billID, txbDateCreate.Text);
                    fPrintBill.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Hóa đơn chưa thanh toán\nBạn không có quyền truy cập", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Click
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            txbSearch.Text = txbSearch.Text.Trim();
            if (txbSearch.Text != string.Empty)
            {
                txbDateCreate.Text = string.Empty;
                txbName.Text = string.Empty;
                txbPrice.Text = string.Empty;
                txbStatusRoom.Text = string.Empty;
                txbUser.Text = string.Empty;

                btnSearch.Visible = false;
                btnCancel.Visible = true;
                Search();
            }
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            LoadFullBill(GetFullBill());
            btnCancel.Visible = false;
            btnSearch.Visible = true;
        }
        #endregion

        #region Method

        private void Search()
        {
            LoadFullBill(GetSearchBill(txbSearch.Text, cbBillSearch.SelectedIndex));
        }
        #endregion

        #region Get Data
        private DataTable GetFullBill()
        {
            return BillDAO.Instance.LoaddFullBill();
        }
        private DataTable GetSearchBill(string text, int mode)
        {
            return BillDAO.Instance.SearchBill(text, mode);
        }



        #endregion

        #region Key
        private void TxbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                BtnSearch_Click(sender, null);
            else
                if (e.KeyChar == 27 && btnCancel.Visible == true)
                BtnCancel_Click(sender, null);
        }
        private void FBill_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27 && btnCancel.Visible == true)
                BtnCancel_Click(sender, null);
        }
        #endregion

        private void txbCustomerName_OnValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewBill_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
