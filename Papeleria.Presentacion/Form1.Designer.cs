namespace Papeleria.Presentacion
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
            btnProductos = new Button();
            btnTestConexion = new Button();
            SuspendLayout();
            btnTestConexion.Location = new Point(20, 20);
            btnTestConexion.Name = "btnTestConexion";
            btnTestConexion.Size = new Size(200, 40);
            btnTestConexion.Text = "Test de conexión";
            btnTestConexion.TabIndex = 0;
            btnTestConexion.UseVisualStyleBackColor = true;
            btnTestConexion.Click += btnTestConexion_Click;
            Controls.Add(btnTestConexion);
            btnProductos.TabIndex = 1;
            btnProductos.Location = new Point(240, 20);
            btnProductos.Name = "btnProductos";
            btnProductos.Size = new Size(200, 40);
            btnProductos.Text = "Abrir productos";
            btnProductos.UseVisualStyleBackColor = true;
            btnProductos.Click += btnProductos_Click;
            Controls.Add(btnProductos);
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 180);
            Name = "Form1";
            Text = "Form1 | Prueba de conexión";
            StartPosition = FormStartPosition.CenterScreen;
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnProductos;
        private Button btnTestConexion;
    }
}
