namespace Lumins_and_Solvers
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
            LevelDataTextBox = new TextBox();
            OutputTextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            GoButton = new Button();
            AllDistinctSolutionsCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // LevelDataTextBox
            // 
            LevelDataTextBox.Font = new Font("DejaVu Sans Mono", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LevelDataTextBox.Location = new Point(12, 34);
            LevelDataTextBox.Multiline = true;
            LevelDataTextBox.Name = "LevelDataTextBox";
            LevelDataTextBox.Size = new Size(374, 371);
            LevelDataTextBox.TabIndex = 0;
            // 
            // OutputTextBox
            // 
            OutputTextBox.Font = new Font("DejaVu Sans Mono", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            OutputTextBox.Location = new Point(414, 34);
            OutputTextBox.Multiline = true;
            OutputTextBox.Name = "OutputTextBox";
            OutputTextBox.Size = new Size(374, 371);
            OutputTextBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(166, 9);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 2;
            label1.Text = "Level Data";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(587, 9);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 3;
            label2.Text = "Output";
            // 
            // GoButton
            // 
            GoButton.Location = new Point(364, 415);
            GoButton.Name = "GoButton";
            GoButton.Size = new Size(75, 23);
            GoButton.TabIndex = 4;
            GoButton.Text = "Go";
            GoButton.UseVisualStyleBackColor = true;
            GoButton.Click += GoButton_Click;
            // 
            // AllDistinctSolutionsCheckBox
            // 
            AllDistinctSolutionsCheckBox.AutoSize = true;
            AllDistinctSolutionsCheckBox.Location = new Point(445, 418);
            AllDistinctSolutionsCheckBox.Name = "AllDistinctSolutionsCheckBox";
            AllDistinctSolutionsCheckBox.Size = new Size(135, 19);
            AllDistinctSolutionsCheckBox.TabIndex = 5;
            AllDistinctSolutionsCheckBox.Text = "All Distinct Solutions";
            AllDistinctSolutionsCheckBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(AllDistinctSolutionsCheckBox);
            Controls.Add(GoButton);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(OutputTextBox);
            Controls.Add(LevelDataTextBox);
            Name = "Form1";
            Text = "Lumins and Solvers";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox LevelDataTextBox;
        private TextBox OutputTextBox;
        private Label label1;
        private Label label2;
        private Button GoButton;
        private CheckBox AllDistinctSolutionsCheckBox;
    }
}
