namespace ChristMembers.Behaviors;

public class MascaraCpfBehavior : Behavior<Entry>
{
    private bool _atualizando;

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        entry.TextChanged += OnTextChanged;
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.TextChanged -= OnTextChanged;
        base.OnDetachingFrom(entry);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_atualizando || sender is not Entry entry)
            return;

        if (entry.Text == Formatar(entry.Text))
            return;

        entry.Dispatcher.Dispatch(() =>
        {
            _atualizando = true;

            string formatado = Formatar(entry.Text);

            if (entry.Text != formatado)
            {
                entry.Text = formatado;
                entry.CursorPosition = formatado.Length;
            }

            _atualizando = false;
        });
    }

    private static string Formatar(string? texto)
    {
        string digitos = new string(
            (texto ?? string.Empty)
                .Where(char.IsDigit)
                .Take(11)
                .ToArray());

        if (digitos.Length > 9)
            return $"{digitos[..3]}.{digitos[3..6]}.{digitos[6..9]}-{digitos[9..]}";

        if (digitos.Length > 6)
            return $"{digitos[..3]}.{digitos[3..6]}.{digitos[6..]}";

        if (digitos.Length > 3)
            return $"{digitos[..3]}.{digitos[3..]}";

        return digitos;
    }
}