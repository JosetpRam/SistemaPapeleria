using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Papeleria.Entidades;
using Papeleria.Negocio;

namespace Papeleria.Presentacion;

// Editor modal: trabaja sobre una copia, nunca sobre la fila enlazada al grid.
public class FormProductoEdicion : XtraForm
{
    private readonly Producto? _original;
    private readonly TextEdit txtCodigo = new();
    private readonly TextEdit txtNombre = new();
    private readonly LookUpEdit cboCategoria = new();
    private readonly SpinEdit spPrecio = new();
    private readonly SpinEdit spMinimo = new();
    private readonly CheckEdit chkActivo = new();
    private readonly SimpleButton btnGuardar = new();
    private readonly SimpleButton btnCancelar = new();
    private readonly LabelControl lblMensaje = new();
    private bool _ocupado;
    private bool _cambio;
    public int ProductoGuardadoId { get; private set; }

    public FormProductoEdicion(Producto? original, List<Categoria> categorias)
    {
        // Construir todo antes de aplicar el escalado de pantalla.
        SuspendLayout();
        _original = original;
        Text = original == null ? "Nuevo producto" : $"Editar producto #{original.ProductoId}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false;
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96, 96);
        ClientSize = new Size(650, 445);
        var titulo = new LabelControl { Text = Text, Location = new Point(24, 20) };
        titulo.Appearance.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        Controls.Add(titulo);
        Agregar("Código *", txtCodigo, 72);
        Agregar("Nombre *", txtNombre, 116);
        Agregar("Categoría *", cboCategoria, 160);
        Agregar("Precio de venta *", spPrecio, 204);
        Agregar("Existencia mínima *", spMinimo, 248);
        txtCodigo.Properties.MaxLength = 30;
        txtNombre.Properties.MaxLength = 150;
        cboCategoria.Properties.DataSource = categorias.Where(c => c.Activo || c.CategoriaId == original?.CategoriaId).ToList();
        cboCategoria.Properties.DisplayMember = nameof(Categoria.Descripcion);
        cboCategoria.Properties.ValueMember = nameof(Categoria.CategoriaId);
        cboCategoria.Properties.NullText = "Seleccione una categoría";
        cboCategoria.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cboCategoria.Properties.Columns.Add(new LookUpColumnInfo(nameof(Categoria.Descripcion), "Categoría"));
        spPrecio.Properties.MinValue = 0;
        spPrecio.Properties.MaxValue = 9999999999999999.99m;
        spPrecio.Properties.IsFloatValue = true;
        spPrecio.Properties.Mask.EditMask = "n2";
        spPrecio.Properties.Mask.UseMaskAsDisplayFormat = true;
        spMinimo.Properties.MinValue = 0;
        spMinimo.Properties.MaxValue = 999999999999999.999m;
        spMinimo.Properties.IsFloatValue = true;
        spMinimo.Properties.Mask.EditMask = "n3";
        spMinimo.Properties.Mask.UseMaskAsDisplayFormat = true;
        chkActivo.Text = "Producto activo";
        chkActivo.Location = new Point(195, 292); chkActivo.Size = new Size(250, 28);
        Controls.Add(chkActivo);
        lblMensaje.Text = "* Obligatorio. La existencia mínima no es el stock actual.";
        lblMensaje.Location = new Point(24, 336); Controls.Add(lblMensaje);
        btnGuardar.Text = "Guardar"; btnGuardar.Location = new Point(365, 382); btnGuardar.Size = new Size(120, 36);
        btnCancelar.Text = "Cancelar"; btnCancelar.Location = new Point(497, 382); btnCancelar.Size = new Size(120, 36);
        Controls.AddRange(new Control[] { btnGuardar, btnCancelar });
        txtCodigo.TabIndex = 0; txtNombre.TabIndex = 1; cboCategoria.TabIndex = 2;
        spPrecio.TabIndex = 3; spMinimo.TabIndex = 4; chkActivo.TabIndex = 5;
        btnGuardar.TabIndex = 6; btnCancelar.TabIndex = 7;
        AcceptButton = btnGuardar; CancelButton = btnCancelar;
        btnCancelar.Click += (_, _) => Close();
        btnGuardar.Click += Guardar_Click;
        txtCodigo.Text = original?.Codigo ?? "";
        txtNombre.Text = original?.Nombre ?? "";
        cboCategoria.EditValue = original?.CategoriaId;
        spPrecio.Value = original?.PrecioVenta ?? 0;
        spMinimo.Value = original?.ExistenciaMinima ?? 0;
        chkActivo.Checked = original?.Activo ?? true;
        foreach (var editor in new BaseEdit[] { txtCodigo, txtNombre, cboCategoria, spPrecio, spMinimo, chkActivo })
            editor.EditValueChanged += (_, _) => _cambio = true;
        FormClosing += (_, e) =>
        {
            if (_ocupado) { e.Cancel = true; return; }
            if (_cambio && DialogResult != DialogResult.OK)
                e.Cancel = XtraMessageBox.Show(this, "¿Descartar los cambios sin guardar?", "Productos",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes;
        };
        ResumeLayout(false);
        PerformLayout();
    }

    private void Agregar(string texto, BaseEdit editor, int y)
    {
        Controls.Add(new LabelControl { Text = texto, Location = new Point(24, y + 5) });
        editor.Location = new Point(195, y); editor.Size = new Size(420, 28);
        editor.TabIndex = y / 44; Controls.Add(editor);
    }

    private async void Guardar_Click(object? sender, EventArgs e)
    {
        if (_ocupado) return;
        if (!ValidateChildren()) return;
        bool guardado = false;
        _ocupado = true; btnGuardar.Enabled = btnCancelar.Enabled = false; UseWaitCursor = true;
        try
        {
            // Captura valores antes de esperar; evita que se cambien durante el envío.
            var p = new Producto
            {
                ProductoId = _original?.ProductoId ?? 0,
                Codigo = txtCodigo.Text,
                Nombre = txtNombre.Text,
                CategoriaId = cboCategoria.EditValue is int id ? id : 0,
                PrecioVenta = spPrecio.Value,
                ExistenciaMinima = spMinimo.Value,
                Activo = chkActivo.Checked
            };
            foreach (var editor in Controls.OfType<BaseEdit>()) editor.Enabled = false;
            string clave = Environment.GetEnvironmentVariable("PAPELERIA_DB_PASSWORD", EnvironmentVariableTarget.User) ?? "";
            ProductoGuardadoId = await new ProductoNegocio().GuardarAsync(clave, p, _original);
            guardado = true; _cambio = false;
        }
        catch (Exception ex)
        {
            string mensaje = ex is ArgumentException or InvalidOperationException ? ex.Message :
                "No se pudo confirmar el guardado. Actualice la lista antes de reintentar. Tipo: " + ex.GetType().Name;
            XtraMessageBox.Show(this, mensaje, "Guardar producto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            _ocupado = false; UseWaitCursor = false;
            btnGuardar.Enabled = btnCancelar.Enabled = true;
            foreach (var editor in Controls.OfType<BaseEdit>()) editor.Enabled = true;
        }
        if (guardado) { DialogResult = DialogResult.OK; Close(); }
    }
}
