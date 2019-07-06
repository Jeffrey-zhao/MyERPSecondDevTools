namespace MyERPSecondDevTools
{
    partial class MyERPSecondDevTools
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyERPSecondDevTools));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tool_ButtonSetERPPath = new System.Windows.Forms.ToolStripButton();
            this.toolStripHistory = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolErpPathLabel = new System.Windows.Forms.ToolStripLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button_fiddler = new System.Windows.Forms.Button();
            this.txt_pageUrl = new System.Windows.Forms.TextBox();
            this.timer_GetResponse = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tool_ButtonSetERPPath,
            this.toolStripHistory,
            this.toolErpPathLabel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1244, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tool_ButtonSetERPPath
            // 
            this.tool_ButtonSetERPPath.Image = global::MyERPSecondDevTools.Properties.Resources.数据目录;
            this.tool_ButtonSetERPPath.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_ButtonSetERPPath.Name = "tool_ButtonSetERPPath";
            this.tool_ButtonSetERPPath.Size = new System.Drawing.Size(98, 22);
            this.tool_ButtonSetERPPath.Text = "设置ERP路径";
            this.tool_ButtonSetERPPath.Click += new System.EventHandler(this.toolSetERPPath_Click);
            // 
            // toolStripHistory
            // 
            this.toolStripHistory.Image = global::MyERPSecondDevTools.Properties.Resources.历史记录;
            this.toolStripHistory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripHistory.Name = "toolStripHistory";
            this.toolStripHistory.Size = new System.Drawing.Size(85, 22);
            this.toolStripHistory.Text = "选择历史";
            // 
            // toolErpPathLabel
            // 
            this.toolErpPathLabel.Name = "toolErpPathLabel";
            this.toolErpPathLabel.Size = new System.Drawing.Size(0, 22);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBox1);
            this.groupBox1.Controls.Add(this.button_fiddler);
            this.groupBox1.Controls.Add(this.txt_pageUrl);
            this.groupBox1.Location = new System.Drawing.Point(0, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1244, 688);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(5, 39);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1233, 642);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // button_fiddler
            // 
            this.button_fiddler.Location = new System.Drawing.Point(1008, 12);
            this.button_fiddler.Name = "button_fiddler";
            this.button_fiddler.Size = new System.Drawing.Size(75, 23);
            this.button_fiddler.TabIndex = 0;
            this.button_fiddler.Text = "Fiddler";
            this.button_fiddler.UseVisualStyleBackColor = true;
            this.button_fiddler.Click += new System.EventHandler(this.button_fiddler_Click);
            // 
            // txt_pageUrl
            // 
            this.txt_pageUrl.Location = new System.Drawing.Point(7, 13);
            this.txt_pageUrl.Name = "txt_pageUrl";
            this.txt_pageUrl.Size = new System.Drawing.Size(992, 21);
            this.txt_pageUrl.TabIndex = 1;
            // 
            // timer_GetResponse
            // 
            this.timer_GetResponse.Interval = 1000;
            this.timer_GetResponse.Tick += new System.EventHandler(this.timer_GetResponse_Tick);
            // 
            // MyERPSecondDevTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1244, 724);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MyERPSecondDevTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "明源ERP二开工具箱 - V1.0.0";
            this.Load += new System.EventHandler(this.MyERPSecondDevTools_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tool_ButtonSetERPPath;
        private System.Windows.Forms.ToolStripLabel toolErpPathLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripHistory;
        private System.Windows.Forms.Button button_fiddler;
        private System.Windows.Forms.TextBox txt_pageUrl;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Timer timer_GetResponse;
    }
}

