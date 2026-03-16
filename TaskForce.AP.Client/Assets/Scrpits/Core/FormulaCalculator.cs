using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core
{
    /// <summary>
    /// 문자열 ID로 등록된 수식(공식)을 관리하고, 계수와 변수를 전달받아 계산을 수행하는 클래스.
    /// </summary>
    public class FormulaCalculator
    {
        /// <summary>ID를 키로 하여 등록된 수식 함수 딕셔너리.</summary>
        private readonly Dictionary<string, Func<IReadOnlyDictionary<string, float>, float[], float>> _formulas;

        /// <summary>경고 및 오류 메시지를 출력하기 위한 로거.</summary>
        private readonly ILogger _logger;

        /// <summary>
        /// <see cref="FormulaCalculator"/>의 새 인스턴스를 생성한다.
        /// </summary>
        /// <param name="logger">로그 출력에 사용할 로거 인스턴스.</param>
        public FormulaCalculator(ILogger logger)
        {
            _formulas = new Dictionary<string, Func<IReadOnlyDictionary<string, float>, float[], float>>();
            _logger = logger;
        }

        /// <summary>
        /// 지정된 ID로 수식 함수를 등록한다. 이미 같은 ID가 존재하면 덮어쓴다.
        /// </summary>
        /// <param name="id">수식을 식별하는 고유 ID.</param>
        /// <param name="formula">계수 딕셔너리와 변수 배열을 받아 결과를 반환하는 수식 함수.</param>
        public void RegisterFormula(string id, Func<IReadOnlyDictionary<string, float>, float[], float> formula)
        {
            _formulas[id] = formula;
        }

        /// <summary>
        /// 등록된 수식을 사용하여 계산을 수행한다. 수식이 등록되어 있지 않으면 첫 번째 변수 값 또는 0을 반환한다.
        /// </summary>
        /// <param name="calculationType">실행할 수식의 ID.</param>
        /// <param name="coefficients">수식에 전달할 계수 딕셔너리.</param>
        /// <param name="variables">수식에 전달할 변수 배열.</param>
        /// <returns>수식 계산 결과 값.</returns>
        public float Calculate(string calculationType, IReadOnlyDictionary<string, float> coefficients, params float[] variables)
        {
            if (!_formulas.TryGetValue(calculationType, out var formula))
            {
                _logger.Warn($"[FormulaCalculator] Formula '{calculationType}' not found.");
                return variables.Length > 0 ? variables[0] : 0f;
            }

            return formula(coefficients, variables);
        }
    }
}
