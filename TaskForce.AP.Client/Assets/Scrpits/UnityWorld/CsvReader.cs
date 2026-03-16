using System.Collections.Generic;
using System.Text;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// CSV 형식의 원본 텍스트 데이터를 파싱하여 헤더-값 딕셔너리 리스트로 변환하는 클래스.
    /// 따옴표로 감싸진 필드, 이중 따옴표, 여러 줄에 걸친 필드 등을 올바르게 처리한다.
    /// </summary>
    public class CsvReader
    {
        /// <summary>로거 인스턴스</summary>
        private readonly Core.ILogger _logger;

        /// <summary>
        /// CsvReader의 새 인스턴스를 초기화한다.
        /// </summary>
        /// <param name="logger">파싱 오류 로깅을 위한 로거 인스턴스</param>
        public CsvReader(Core.ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// CSV 원본 텍스트를 파싱하여 각 행을 헤더-값 딕셔너리 형태의 리스트로 반환한다.
        /// 첫 번째 줄을 헤더로 사용하며, 컬럼 수가 맞지 않는 행은 건너뛴다.
        /// </summary>
        /// <param name="rawCsvData">파싱할 CSV 원본 텍스트</param>
        /// <returns>각 행이 헤더 키와 값의 딕셔너리인 리스트</returns>
        public List<Dictionary<string, string>> ReadCsvData(string rawCsvData)
        {
            var result = new List<Dictionary<string, string>>();
            if (string.IsNullOrEmpty(rawCsvData))
            {
                _logger.Fatal("CSV data is empty.");
                return result;
            }

            var lines = SplitCsvLines(rawCsvData);
            if (lines.Count < 2)
            {
                _logger.Fatal("CSV has no header or data.");
                return result;
            }

            var headers = ParseCsvLine(lines[0]);
            for (int i = 1; i < lines.Count; i++)
            {
                var fields = ParseCsvLine(lines[i]);
                if (fields.Count != headers.Count)
                {
                    _logger.Warn($"Skipping line {i + 1}: Column count mismatch (Expected {headers.Count}, Actual {fields.Count})");
                    continue;
                }

                var row = new Dictionary<string, string>();
                for (int j = 0; j < headers.Count; j++)
                {
                    row[headers[j]] = fields[j];
                }
                result.Add(row);
            }

            return result;
        }

        /// <summary>
        /// CSV 원본 텍스트를 줄 단위로 분리한다. 따옴표 내부의 줄바꿈은 무시한다.
        /// </summary>
        /// <param name="rawCsv">분리할 CSV 원본 텍스트</param>
        /// <returns>줄 단위로 분리된 문자열 리스트</returns>
        private List<string> SplitCsvLines(string rawCsv)
        {
            var lines = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < rawCsv.Length; i++)
            {
                char c = rawCsv[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    sb.Append(c);
                }
                else if ((c == '\n' || c == '\r') && !inQuotes)
                {
                    if (sb.Length > 0)
                    {
                        lines.Add(sb.ToString().TrimEnd('\r'));
                        sb.Clear();
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
                lines.Add(sb.ToString());

            return lines;
        }

        /// <summary>
        /// CSV 한 줄을 파싱하여 각 필드 값의 리스트를 반환한다.
        /// 따옴표로 감싸진 필드와 이중 따옴표 이스케이프를 처리한다.
        /// </summary>
        /// <param name="line">파싱할 CSV 한 줄</param>
        /// <returns>필드 값 리스트</returns>
        private List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"'); // 이중 따옴표는 하나의 따옴표로 처리
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        fields.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            fields.Add(sb.ToString());
            return fields;
        }
    }
}
