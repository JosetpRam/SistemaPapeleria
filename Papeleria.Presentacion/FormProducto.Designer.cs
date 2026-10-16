namespace Papeleria.Presentacion
{
    partial class FormProducto
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelCabecera = new DevExpress.XtraEditors.PanelControl();
            lblTitulo = new DevExpress.XtraEditors.LabelControl();
            lblDescripcion = new DevExpress.XtraEditors.LabelControl();
            btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            btnEditar = new DevExpress.XtraEditors.SimpleButton();
            btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            btnActualizar = new DevExpress.XtraEditors.SimpleButton();
            gridProductos = new DevExpress.XtraGrid.GridControl();
            vistaProductos = new DevExpress.XtraGrid.Views.Grid.GridView();
            lblEstado = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)panelCabecera).BeginInit();
            panelCabecera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridProductos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)vistaProductos).BeginInit();
            SuspendLayout();

            panelCabecera.Controls.Add(lblTitulo);
            panelCabecera.Controls.Add(lblDescripcion);
            panelCabecera.Controls.Add(btnActualizar);
            panelCabecera.Dock = DockStyle.Top;
            panelCabecera.Name = "panelCabecera";
            panelCabecera.Size = new Size(1100, 84);

            lblTitulo.Appearance.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitulo.Appearance.Options.UseFont = true;
            lblTitulo.Location = new Point(18, 10);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Text = "Productos";

            lblDescripcion.Location = new Point(20, 50);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Text = "Administración de productos | Nuevo, editar y eliminar";

            btnNuevo.Name = "btnNuevo";
            btnNuevo.Text = "Nuevo";
            btnNuevo.Location = new Point(550, 22);
            btnNuevo.Size = new Size(120, 36);
            btnNuevo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNuevo.Click += btnNuevo_Click;
            panelCabecera.Controls.Add(btnNuevo);
            btnEditar.Name = "btnEditar";
            btnEditar.Text = "Editar";
            btnEditar.Location = new Point(682, 22);
            btnEditar.Size = new Size(120, 36);
            btnEditar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEditar.Click += btnEditar_Click;
            panelCabecera.Controls.Add(btnEditar);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Text = "Eliminar";
            btnEliminar.Location = new Point(814, 22);
            btnEliminar.Size = new Size(120, 36);
            btnEliminar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEliminar.Click += btnEliminar_Click;
            panelCabecera.Controls.Add(btnEliminar);

            btnActualizar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnActualizar.Location = new Point(946, 22);
            btnActualizar.Name = "btnActualizar";
            btnActualizar.Size = new Size(132, 36);
            btnActualizar.Text = "Actualizar";
            btnActualizar.TabIndex = 0;
            btnActualizar.Click += btnActualizar_Click;

            gridProductos.Dock = DockStyle.Fill;
            gridProductos.MainView = vistaProductos;
            gridProductos.Name = "gridProductos";
            gridProductos.TabIndex = 1;
            gridProductos.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { vistaProductos });

            vistaProductos.GridControl = gridProductos;
            vistaProductos.Name = "vistaProductos";
            vistaProductos.OptionsBehavior.AutoPopulateColumns = false;
            vistaProductos.OptionsBehavior.Editable = false;
            vistaProductos.OptionsView.ShowGroupPanel = false;
            vistaProductos.OptionsView.ShowAutoFilterRow = true;
            vistaProductos.OptionsView.EnableAppearanceEvenRow = true;
            vistaProductos.OptionsFind.AlwaysVisible = true;
            vistaProductos.OptionsFind.FindNullPrompt = "Buscar productos...";

            lblEstado.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblEstado.Dock = DockStyle.Bottom;
            lblEstado.Name = "lblEstado";
            lblEstado.Padding = new Padding(12, 0, 12, 0);
            lblEstado.Size = new Size(1100, 36);
            lblEstado.Text = "Listo para consultar.";

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 620);
            Controls.Add(gridProductos);
            Controls.Add(lblEstado);
            Controls.Add(panelCabecera);
            MinimumSize = new Size(1100, 550);
            Name = "FormProducto";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sistema Papelería | Productos";
            Load += FormProducto_Load;
            ((System.ComponentModel.ISupportInitialize)panelCabecera).EndInit();
            panelCabecera.ResumeLayout(false);
            panelCabecera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridProductos).EndInit();
            ((System.ComponentModel.ISupportInitialize)vistaProductos).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private DevExpress.XtraEditors.PanelControl panelCabecera;
        private DevExpress.XtraEditors.LabelControl lblTitulo;
        private DevExpress.XtraEditors.LabelControl lblDescripcion;
        private DevExpress.XtraEditors.SimpleButton btnActualizar;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnEditar;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraGrid.GridControl gridProductos;
        private DevExpress.XtraGrid.Views.Grid.GridView vistaProductos;
        private DevExpress.XtraEditors.LabelControl lblEstado;
    }
}
