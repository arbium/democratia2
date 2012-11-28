namespace Federation.Scheduler.Tests
{
    partial class SchedulerForm
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
            this.butStart = new System.Windows.Forms.Button();
            this.butStop = new System.Windows.Forms.Button();
            this.butTask1 = new System.Windows.Forms.Button();
            this.butTask2 = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.grbTasks = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grbTasks.SuspendLayout();
            this.SuspendLayout();
            // 
            // butStart
            // 
            this.butStart.Location = new System.Drawing.Point(12, 33);
            this.butStart.Name = "butStart";
            this.butStart.Size = new System.Drawing.Size(94, 23);
            this.butStart.TabIndex = 0;
            this.butStart.Text = "Запустить";
            this.butStart.UseVisualStyleBackColor = true;
            this.butStart.Click += new System.EventHandler(this.butStart_Click);
            // 
            // butStop
            // 
            this.butStop.Location = new System.Drawing.Point(12, 62);
            this.butStop.Name = "butStop";
            this.butStop.Size = new System.Drawing.Size(94, 23);
            this.butStop.TabIndex = 1;
            this.butStop.Text = "Остановить";
            this.butStop.UseVisualStyleBackColor = true;
            this.butStop.Click += new System.EventHandler(this.butStop_Click);
            // 
            // butTask1
            // 
            this.butTask1.Location = new System.Drawing.Point(6, 19);
            this.butTask1.Name = "butTask1";
            this.butTask1.Size = new System.Drawing.Size(259, 23);
            this.butTask1.TabIndex = 2;
            this.butTask1.Text = "Задача \"Записать в файл\"";
            this.butTask1.UseVisualStyleBackColor = true;
            this.butTask1.Click += new System.EventHandler(this.butTask1_Click);
            // 
            // butTask2
            // 
            this.butTask2.Location = new System.Drawing.Point(6, 48);
            this.butTask2.Name = "butTask2";
            this.butTask2.Size = new System.Drawing.Size(259, 23);
            this.butTask2.TabIndex = 3;
            this.butTask2.Text = "Задача \"\"";
            this.butTask2.UseVisualStyleBackColor = true;
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(342, 22);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(219, 20);
            this.txtFileName.TabIndex = 4;
            // 
            // grbTasks
            // 
            this.grbTasks.Controls.Add(this.label1);
            this.grbTasks.Controls.Add(this.butTask1);
            this.grbTasks.Controls.Add(this.txtFileName);
            this.grbTasks.Controls.Add(this.butTask2);
            this.grbTasks.Location = new System.Drawing.Point(112, 12);
            this.grbTasks.Name = "grbTasks";
            this.grbTasks.Size = new System.Drawing.Size(577, 276);
            this.grbTasks.TabIndex = 5;
            this.grbTasks.TabStop = false;
            this.grbTasks.Text = "Задачи";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(269, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Имя файла:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.Green;
            this.lblStatus.Location = new System.Drawing.Point(30, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(54, 13);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Работает";
            // 
            // SchedulerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 300);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.grbTasks);
            this.Controls.Add(this.butStop);
            this.Controls.Add(this.butStart);
            this.Name = "SchedulerForm";
            this.Text = "Тестирование шедуллера";
            this.grbTasks.ResumeLayout(false);
            this.grbTasks.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button butStart;
        private System.Windows.Forms.Button butStop;
        private System.Windows.Forms.Button butTask1;
        private System.Windows.Forms.Button butTask2;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.GroupBox grbTasks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatus;
    }
}

