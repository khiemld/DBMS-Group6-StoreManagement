﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrepareForFinal.BSLayer;
using RestSharp.Extensions;

namespace PrepareForFinal.UI
{
    public partial class us_customerUI : UserControl
    {
        public us_customerUI()
        {
            InitializeComponent();
            loadData();
        }

        DataTable dtCustomer = new DataTable();
        Customer dbCustomer = new Customer();

        bool addFlag;

        void loadData()
        {
            try
            {
                dtCustomer = new DataTable();
                dtCustomer.Clear();
                DataSet ds = dbCustomer.GetCustomer();
                //Đưa dữ liệu vào Database
                dtCustomer = ds.Tables[0];

                //Đưa dữ liệu lên DataGridView từ DataTable
                dtgv_customerList.DataSource = dtCustomer;
                dtgv_customerList.AutoResizeColumnHeadersHeight();

                setDataGridView();
            }
            catch
            {
                MessageBox.Show("Không load được dữ liệu", MessageBoxIcon.Error.ToString());
            }
            btn_customerDelete.Enabled = false;
        }
        
        void setDataGridView()
        {
            if(dtgv_customerList != null)
            {
                //Set Header Text cho dtgv
                dtgv_customerList.Columns[0].HeaderText = "ID";
                dtgv_customerList.Columns[1].HeaderText = "Tên";
                dtgv_customerList.Columns[2].HeaderText = "Nam";
                dtgv_customerList.Columns[3].HeaderText = "Sinh nhật";
                dtgv_customerList.Columns[4].HeaderText = "Địa chỉ";
                dtgv_customerList.Columns[5].HeaderText = "Phone";
                dtgv_customerList.Columns[6].HeaderText = "Điểm";

                //Ẩn các cột: sinh nhật, địa chỉ, trang thái.
                for (int i = 0; i < dtgv_customerList.ColumnCount; i++)
                {
                    if (i == 3 || i == 4 || i == 7)
                        dtgv_customerList.Columns[i].Visible = false;
                }

                //Set chiều rộng cột
                int width = dtgv_customerList.Width;
                int n_column = dtgv_customerList.ColumnCount;
                dtgv_customerList.Columns[0].Width -= width / n_column;
                dtgv_customerList.Columns[1].Width -= width / n_column;
                dtgv_customerList.Columns[2].Width -= width / n_column;
                dtgv_customerList.Columns[3].Width -= width / n_column;
                dtgv_customerList.Columns[4].Width -= width / n_column;
                dtgv_customerList.Columns[5].Width -= width / n_column;
                dtgv_customerList.Columns[6].Width -= width / n_column;

                dtgv_customerList.AutoResizeColumns();
            }
        }

        private void us_customerUI_Load(object sender, EventArgs e)
        {
            txt_customerPoint.Enabled = false;
            btn_customerSave.Enabled = false;
            btn_customerCancel.Enabled = false;
        }

        private void btn_customerAdd_Click(object sender, EventArgs e)
        {
            addFlag = true;
            btn_customerAdd.Enabled = false;
            btn_customerSave.Enabled = true;
            btn_customerCancel.Enabled = true;
            btn_customerUpdate.Enabled =false;
        }

        private void btn_customerUpdate_Click(object sender, EventArgs e)
        {
            //Trường hợp khi người dùng chưa chọn Khách hàng cần sửa thông tin
            if(txt_customerID.Text == "" || txt_customerID.Text == null)
            {
                MessageBox.Show("Vui lòng chọn Khách hàng cần chỉnh sửa thông tin\n" +
                    "Hoặc nhập ID", "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            btn_customerAdd.Enabled = false;
            btn_customerSave.Enabled = true;
            btn_customerCancel.Enabled = true;
            btn_customerUpdate.Enabled = false;
            txt_customerID.Enabled = false;
        }

        private void btn_customerSave_Click(object sender, EventArgs e)
        {
            if(addFlag == true) //Trường hợp thêm Khách hàng
            {
                try
                {
                    //Lấy giới tính
                    int isMale = rb_customerMale.Checked ? 1 : 0;

                    dbCustomer.addCustomer(
                        txt_customerID.Text.Trim(),
                        txt_customerName.Text.Trim(),
                        isMale,
                        dtp_customerBirthdate.Value,
                        txt_customerAddress.Text.Trim(),
                        txt_customerPhone.Text.Trim(),
                        //Lấy giá trị điểm trong Textbox, nếu textbox không có dữ liệu thì cho nó bằng 0
                        txt_customerPoint.Text.Trim() != null ? (int)Convert.ToInt32(txt_customerPoint.Text.Trim()) : 0);

                    MessageBox.Show("Thêm khách hàng thành công");
                    addFlag = false;
                    loadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else //Trường hợp người dùng nhấn update
            {
                try
                {
                    //Lấy giới tính
                    int isMale = rb_customerMale.Checked ? 1 : 0;
                    dbCustomer.updateCustomer(
                        txt_customerID.Text.Trim(),
                        txt_customerName.Text.Trim(),
                        isMale,
                        dtp_customerBirthdate.Value,
                        txt_customerAddress.Text.Trim(),
                        txt_customerPhone.Text.Trim(),
                        //Lấy giá trị điểm trong Textbox, nếu textbox không có dữ liệu thì cho nó bằng 0
                        txt_customerPoint.Text.Trim() != null ? (int)Convert.ToInt32(txt_customerPoint.Text.Trim()) : 0,
                        0);

                    MessageBox.Show("Cập nhật thông tin khách hàng thành công");
                    loadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            btn_customerAdd.Enabled = true;
            btn_customerSave.Enabled = false;
            btn_customerCancel.Enabled = false;
            btn_customerUpdate.Enabled=true;
            txt_customerID.Enabled = true;
        }

        private void btn_customerCancel_Click(object sender, EventArgs e)
        {
            btn_customerAdd.Enabled = true;
            btn_customerSave.Enabled = false;
            btn_customerCancel.Enabled = false;
            btn_customerUpdate.Enabled = true;
            txt_customerID.Enabled = true;
        }

        private void dtgv_customerList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Lấy vị trí dòng được chọn
            int index = dtgv_customerList.CurrentCell.RowIndex;

            //Load thông tin từ Cell lên các Textbox
            this.txt_customerID.Text = dtgv_customerList.Rows[index].Cells[0].Value.ToString();
            this.txt_customerName.Text = dtgv_customerList.Rows[index].Cells[1].Value.ToString();
            this.dtp_customerBirthdate.Value = Convert.ToDateTime(dtgv_customerList.Rows[index].Cells[3].Value.ToString());
            this.txt_customerAddress.Text = dtgv_customerList.Rows[index].Cells[4].Value.ToString();
            this.txt_customerPhone.Text = dtgv_customerList.Rows[index].Cells[5].Value.ToString();
            this.txt_customerPoint.Text = dtgv_customerList.Rows[index].Cells[6].Value.ToString();
            //Lấy giới tính và gán lên RadioButton
            rb_customerMale.Checked = Convert.ToInt32(dtgv_customerList.Rows[index].Cells[2].Value) == 1 ? true : false;
            rb_customerFemale.Checked = !rb_customerMale.Checked;

            //Cho phép xóa khách hàng
            btn_customerDelete.Enabled = true;
        }

        private void btn_findCustomer_Click(object sender, EventArgs e)
        {
            if(txt_findCustomer.Text != "" && txt_findCustomer.Text != null)
            {
                try
                {
                    //Lấy dữ liệu
                    DataTable dtCustomer = new DataTable();
                    dtCustomer.Clear();
                    dtCustomer = dbCustomer.findCustomer(txt_findCustomer.Text.Trim()).Tables[0];

                    //Load lại dữ liệu
                    dtgv_customerList.DataSource = dtCustomer;
                    setDataGridView();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_customerDelete_Click(object sender, EventArgs e)
        {
            //kiểm tra khách hàng đã được chọn chưa
            if(txt_customerID.Text == "" || txt_customerID.Text == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //Hỏi người dùng là có chắc chắn muốn xóa khách hàng không
                DialogResult respone = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng", "Thông báo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                //Nếu đồng ý
                if (respone == DialogResult.Yes)
                {
                    try
                    {
                        dbCustomer.deleteCustomer(txt_customerID.Text);

                        loadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }
}
