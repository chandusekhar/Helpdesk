using System.Text;

namespace Helpdesk.Infrastructure
{
    public static class CSVHelper
    {
        public static string[] SplitCSVLine(string line)
        {
            List<string> cols = new List<string>();
            bool quoted = false;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (quoted)
                {
                    if (c == '"')
                    {
                        // "" escapes and returns a single "
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i += 2;
                        }
                        else
                        {
                            quoted = false;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    // only enter quoted mode if the " appears when we have not added any characters to the column.
                    if (c == '"' && sb.Length == 0)
                    {
                        quoted = true;
                    }
                    // , splits fields
                    else if (c == ',')
                    {
                        cols.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            // when we've run out of chars, sb will contain the last item we were parsing.
            cols.Add(sb.ToString());
            sb.Clear();

            return cols.ToArray();            
        }

        
    }
}
