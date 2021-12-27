namespace UserSettings
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxPrivate = new System.Windows.Forms.TextBox();
            this.TextBoxPublic = new System.Windows.Forms.TextBox();
            this.TextBoxEncoded = new System.Windows.Forms.TextBox();
            this.TextBoxExported = new System.Windows.Forms.TextBox();
            this.TextBoxDecodedPrivate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ButtonEncode = new System.Windows.Forms.Button();
            this.ButtonDecode = new System.Windows.Forms.Button();
            this.TextBoxDecodedPublic = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Encode";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Decode";
            // 
            // TextBoxPrivate
            // 
            this.TextBoxPrivate.Location = new System.Drawing.Point(66, 28);
            this.TextBoxPrivate.Name = "TextBoxPrivate";
            this.TextBoxPrivate.Size = new System.Drawing.Size(394, 23);
            this.TextBoxPrivate.TabIndex = 2;
            // 
            // TextBoxPublic
            // 
            this.TextBoxPublic.Location = new System.Drawing.Point(66, 57);
            this.TextBoxPublic.Name = "TextBoxPublic";
            this.TextBoxPublic.Size = new System.Drawing.Size(394, 23);
            this.TextBoxPublic.TabIndex = 3;
            // 
            // TextBoxEncoded
            // 
            this.TextBoxEncoded.Location = new System.Drawing.Point(66, 86);
            this.TextBoxEncoded.Name = "TextBoxEncoded";
            this.TextBoxEncoded.Size = new System.Drawing.Size(394, 23);
            this.TextBoxEncoded.TabIndex = 4;
            // 
            // TextBoxExported
            // 
            this.TextBoxExported.Location = new System.Drawing.Point(66, 184);
            this.TextBoxExported.Name = "TextBoxExported";
            this.TextBoxExported.Size = new System.Drawing.Size(394, 23);
            this.TextBoxExported.TabIndex = 5;
            // 
            // TextBoxDecodedPrivate
            // 
            this.TextBoxDecodedPrivate.Location = new System.Drawing.Point(66, 213);
            this.TextBoxDecodedPrivate.Name = "TextBoxDecodedPrivate";
            this.TextBoxDecodedPrivate.Size = new System.Drawing.Size(394, 23);
            this.TextBoxDecodedPrivate.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Private";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Public";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Exported";
            // 
            // ButtonEncode
            // 
            this.ButtonEncode.Location = new System.Drawing.Point(16, 85);
            this.ButtonEncode.Name = "ButtonEncode";
            this.ButtonEncode.Size = new System.Drawing.Size(44, 23);
            this.ButtonEncode.TabIndex = 10;
            this.ButtonEncode.Text = "Go";
            this.ButtonEncode.UseVisualStyleBackColor = true;
            this.ButtonEncode.Click += new System.EventHandler(this.ButtonEncode_Click);
            // 
            // ButtonDecode
            // 
            this.ButtonDecode.Location = new System.Drawing.Point(12, 274);
            this.ButtonDecode.Name = "ButtonDecode";
            this.ButtonDecode.Size = new System.Drawing.Size(44, 23);
            this.ButtonDecode.TabIndex = 11;
            this.ButtonDecode.Text = "Go";
            this.ButtonDecode.UseVisualStyleBackColor = true;
            this.ButtonDecode.Click += new System.EventHandler(this.ButtonDecode_Click);
            // 
            // TextBoxDecodedPublic
            // 
            this.TextBoxDecodedPublic.Location = new System.Drawing.Point(66, 242);
            this.TextBoxDecodedPublic.Name = "TextBoxDecodedPublic";
            this.TextBoxDecodedPublic.Size = new System.Drawing.Size(394, 23);
            this.TextBoxDecodedPublic.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 221);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Private";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 245);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "Public";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 309);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TextBoxDecodedPublic);
            this.Controls.Add(this.ButtonDecode);
            this.Controls.Add(this.ButtonEncode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TextBoxDecodedPrivate);
            this.Controls.Add(this.TextBoxExported);
            this.Controls.Add(this.TextBoxEncoded);
            this.Controls.Add(this.TextBoxPublic);
            this.Controls.Add(this.TextBoxPrivate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox TextBoxPrivate;
        private TextBox TextBoxPublic;
        private TextBox TextBoxEncoded;
        private TextBox TextBoxExported;
        private TextBox TextBoxDecodedPrivate;
        private Label label3;
        private Label label4;
        private Label label5;
        private Button ButtonEncode;
        private Button ButtonDecode;
        private TextBox TextBoxDecodedPublic;
        private Label label6;
        private Label label7;
    }
}