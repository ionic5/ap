using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 속성 변경 스킬 클래스. 소유 유닛의 속성을 변경하는 효과(ModifyAttributeEffect)를 관리하며,
    /// 레벨 변경 시 효과를 재생성하여 소유자에게 적용한다.
    /// </summary>
    public class ModifyAttributeSkill : Skill
    {
        /// <summary>현재 적용 중인 속성 변경 효과 목록.</summary>
        private readonly List<IModifyAttributeEffect> _effects;

        /// <summary>이 스킬에 연결된 효과 데이터 목록.</summary>
        private readonly IEnumerable<GameData.ModifyAttributeSkill> _effectDatas;

        /// <summary>효과 ID와 레벨로 속성 변경 효과를 생성하는 팩토리 함수.</summary>
        private readonly Func<string, int, IModifyAttributeEffect> _createEffect;

        /// <summary>
        /// ModifyAttributeSkill의 생성자.
        /// </summary>
        /// <param name="skillID">스킬 고유 식별자.</param>
        /// <param name="skillData">게임 데이터에서 가져온 스킬 정보.</param>
        /// <param name="textStore">텍스트 리소스 저장소.</param>
        /// <param name="effectDatas">이 스킬에 연결된 효과 데이터 컬렉션.</param>
        /// <param name="createEffect">효과 ID와 레벨을 받아 속성 변경 효과를 생성하는 함수.</param>
        public ModifyAttributeSkill(string skillID, GameData.Skill skillData,
            TextStore textStore, IEnumerable<GameData.ModifyAttributeSkill> effectDatas,
            Func<string, int, IModifyAttributeEffect> createEffect)
            : base(skillID, skillData, textStore)
        {
            _effectDatas = effectDatas;
            _createEffect = createEffect;
            _effects = new List<IModifyAttributeEffect>();
        }

        /// <summary>
        /// 스킬 레벨을 설정한다. 소유자가 있을 경우 기존 효과를 제거하고 새로 생성한 효과를 적용한다.
        /// </summary>
        /// <param name="value">설정할 레벨 값.</param>
        public override void SetLevel(int value)
        {
            base.SetLevel(value);

            var owner = GetOwner();
            if (owner != null)
            {
                owner.RemoveModifyAttributeEffects(_effects);
                RecreateEffects();
                owner.AddModifyAttributeEffects(_effects);
            }
            else
            {
                RecreateEffects();
            }
        }

        /// <summary>
        /// 효과 목록을 초기화하고 효과 데이터를 기반으로 효과를 새로 생성한다.
        /// </summary>
        private void RecreateEffects()
        {
            _effects.Clear();

            foreach (var entry in _effectDatas)
            {
                var effectID = entry.ModifyAttributeEffectID;
                var newEff = _createEffect.Invoke(effectID, 1);
                _effects.Add(newEff);
            }
        }

        /// <summary>
        /// 이 스킬의 속성 변경 효과를 소유 유닛에 적용한다.
        /// </summary>
        public override void AddToOwner()
        {
            var owner = GetOwner();
            owner.AddModifyAttributeEffects(_effects);
        }
    }
}
