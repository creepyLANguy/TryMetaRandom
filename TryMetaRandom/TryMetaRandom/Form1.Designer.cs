namespace TryMetaRandom
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
      this.components = new System.ComponentModel.Container();
      this.btn_previous = new System.Windows.Forms.Button();
      this.btn_next = new System.Windows.Forms.Button();
      this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.btn_inspect = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // btn_previous
      // 
      this.btn_previous.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.btn_previous.Location = new System.Drawing.Point(236, 915);
      this.btn_previous.Name = "btn_previous";
      this.btn_previous.Size = new System.Drawing.Size(159, 59);
      this.btn_previous.TabIndex = 1;
      this.btn_previous.Text = "<";
      this.btn_previous.UseVisualStyleBackColor = true;
      this.btn_previous.Click += new System.EventHandler(this.btn_previous_Click);
      // 
      // btn_next
      // 
      this.btn_next.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.btn_next.Location = new System.Drawing.Point(572, 915);
      this.btn_next.Name = "btn_next";
      this.btn_next.Size = new System.Drawing.Size(165, 59);
      this.btn_next.TabIndex = 2;
      this.btn_next.Text = ">";
      this.btn_next.UseVisualStyleBackColor = true;
      this.btn_next.Click += new System.EventHandler(this.btn_next_Click);
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.Location = new System.Drawing.Point(12, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(996, 898);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // btn_inspect
      // 
      this.btn_inspect.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.btn_inspect.Location = new System.Drawing.Point(401, 915);
      this.btn_inspect.Name = "btn_inspect";
      this.btn_inspect.Size = new System.Drawing.Size(165, 59);
      this.btn_inspect.TabIndex = 3;
      this.btn_inspect.Text = "🔍";
      this.btn_inspect.UseVisualStyleBackColor = true;
      this.btn_inspect.Click += new System.EventHandler(this.btn_inspect_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1022, 986);
      this.Controls.Add(this.btn_inspect);
      this.Controls.Add(this.btn_next);
      this.Controls.Add(this.btn_previous);
      this.Controls.Add(this.pictureBox1);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button btn_previous;
    private System.Windows.Forms.Button btn_next;
    private System.Windows.Forms.BindingSource bindingSource1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button btn_inspect;
  }
}

