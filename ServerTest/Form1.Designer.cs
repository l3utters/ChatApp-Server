namespace ServerTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UserList = new System.Windows.Forms.ListBox();
            this.Online = new System.Windows.Forms.Label();
            this.OnlineCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UserList
            // 
            this.UserList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UserList.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserList.FormattingEnabled = true;
            this.UserList.ItemHeight = 24;
            this.UserList.Location = new System.Drawing.Point(12, 32);
            this.UserList.Name = "UserList";
            this.UserList.Size = new System.Drawing.Size(363, 196);
            this.UserList.TabIndex = 0;
            // 
            // Online
            // 
            this.Online.AutoSize = true;
            this.Online.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Online.Location = new System.Drawing.Point(12, 9);
            this.Online.Name = "Online";
            this.Online.Size = new System.Drawing.Size(104, 20);
            this.Online.TabIndex = 1;
            this.Online.Text = "Users Online:";
            // 
            // OnlineCount
            // 
            this.OnlineCount.AutoSize = true;
            this.OnlineCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OnlineCount.Location = new System.Drawing.Point(122, 9);
            this.OnlineCount.Name = "OnlineCount";
            this.OnlineCount.Size = new System.Drawing.Size(18, 20);
            this.OnlineCount.TabIndex = 2;
            this.OnlineCount.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 241);
            this.Controls.Add(this.OnlineCount);
            this.Controls.Add(this.Online);
            this.Controls.Add(this.UserList);
            this.Name = "Form1";
            this.Text = "Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox UserList;
        private System.Windows.Forms.Label Online;
        private System.Windows.Forms.Label OnlineCount;
    }
}

