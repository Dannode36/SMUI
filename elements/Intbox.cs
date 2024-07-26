namespace SMUI.Elements
{
    public class Intbox : Textbox
    {
        public int Value
        {
            get => int.TryParse(String, out int value) ? value : 0;
            set => String = value.ToString();
        }

        public bool IsValid => int.TryParse(String, out _);

        public int Interval { get; set; } = 1;

        /// <inheritdoc />
        protected override void ReceiveInput(string str)
        {
            bool valid = true;
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];
                if (!char.IsDigit(c) && !(c == '-' && String == "" && i == 0))
                {
                    valid = false;
                    break;
                }
            }
            if (!valid)
                return;

            String += str;
            Callback?.Invoke(this);
        }
    }
}
