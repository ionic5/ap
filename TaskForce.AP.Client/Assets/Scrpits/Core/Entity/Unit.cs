using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace TaskForce.AP.Client.Core.Entity
{
    /// <summary>
    /// 게임 내 유닛을 나타내는 클래스. 체력, 레벨, 경험치, 속성, 스킬, 위치 등
    /// 유닛의 상태를 관리하며, 데미지/치유/경험치 획득 등의 동작을 처리한다.
    /// </summary>
    public class Unit
    {
        /// <summary>경험치가 변경되었을 때 발생하는 이벤트.</summary>
        public event EventHandler ExpChangedEvent;

        /// <summary>필요 경험치가 변경되었을 때 발생하는 이벤트.</summary>
        public event EventHandler RequireExpChangedEvent;

        /// <summary>레벨업했을 때 발생하는 이벤트.</summary>
        public event EventHandler LevelUpEvent;

        /// <summary>유닛이 사망했을 때 발생하는 이벤트.</summary>
        public event EventHandler DiedEvent;

        /// <summary>스킬이 추가되었을 때 발생하는 이벤트.</summary>
        public event EventHandler<SkillAddedEventArgs> SkillAddedEvent;

        /// <summary>게임 데이터 저장소.</summary>
        private readonly GameDataStore _gameDataStore;

        /// <summary>유닛의 현재 레벨.</summary>
        private int _level;

        /// <summary>유닛의 현재 경험치.</summary>
        private int _exp;

        /// <summary>유닛의 현재 체력.</summary>
        private int _hp;

        /// <summary>유닛 바디 리소스 ID.</summary>
        private string _unitBodyID;

        /// <summary>유닛 로직 ID.</summary>
        private string _unitLogicID;

        /// <summary>플레이어 진영 여부.</summary>
        private bool _isPlayerSide;

        /// <summary>유닛의 현재 위치.</summary>
        private Vector2 _position;

        /// <summary>기본 스킬 ID.</summary>
        private string _defaultSkillID;

        /// <summary>유닛의 속성을 관리하는 속성 중재자.</summary>
        private readonly AttributeMediator _attributeMediator;

        /// <summary>유닛이 보유한 활성 스킬 딕셔너리.</summary>
        private readonly Dictionary<string, IActiveSkill> _skills;

        /// <summary>
        /// Unit의 생성자.
        /// </summary>
        /// <param name="gameDataStore">게임 데이터 저장소.</param>
        /// <param name="attributeMediator">속성 중재자.</param>
        public Unit(GameDataStore gameDataStore, AttributeMediator attributeMediator)
        {
            _gameDataStore = gameDataStore;

            _skills = new Dictionary<string, IActiveSkill>();
            _attributeMediator = attributeMediator;
        }

        /// <summary>
        /// 유닛의 속성 성장 공식을 설정한다.
        /// </summary>
        /// <param name="formulas">적용할 성장 공식 컬렉션.</param>
        public void SetAttributeGrowthFormulas(IEnumerable<GameData.GrowthFormula> formulas)
        {
            _attributeMediator.SetGrowthFormulas(formulas);
        }

        /// <summary>
        /// 유닛의 기본 속성을 설정한다.
        /// </summary>
        /// <param name="baseAttributes">속성 ID를 키로 하는 기본 속성 딕셔너리.</param>
        public void SetBaseAttributes(IReadOnlyDictionary<string, Attribute> baseAttributes)
        {
            _attributeMediator.SetBaseAttributes(baseAttributes);
        }

        /// <summary>
        /// 지정된 ID에 해당하는 최종 속성 값을 반환한다.
        /// </summary>
        /// <param name="id">조회할 속성의 고유 식별자.</param>
        /// <returns>최종 계산된 <see cref="Attribute"/> 객체.</returns>
        public Attribute GetAttribute(string id)
        {
            return _attributeMediator.GetAttribute(id);
        }

        /// <summary>
        /// 유닛의 현재 체력을 반환한다.
        /// </summary>
        /// <returns>현재 체력 값.</returns>
        public int GetHP()
        {
            return _hp;
        }

        /// <summary>
        /// 유닛에 데미지를 적용한다. 체력이 0 이하가 되면 사망 이벤트를 발생시킨다.
        /// </summary>
        /// <param name="damage">적용할 데미지 양.</param>
        public void ApplyDamage(int damage)
        {
            var hp = GetHP();
            hp -= damage;
            if (hp < 0)
                hp = 0;

            SetHP(hp);

            if (hp == 0)
                DiedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 유닛이 살아있는지 확인한다.
        /// </summary>
        /// <returns>체력이 0보다 크면 true.</returns>
        public bool IsAlive()
        {
            return GetHP() > 0;
        }

        /// <summary>
        /// 유닛이 사망했는지 확인한다.
        /// </summary>
        /// <returns>체력이 0 이하이면 true.</returns>
        public bool IsDead()
        {
            return !IsAlive();
        }

        /// <summary>
        /// 유닛이 플레이어 진영인지 확인한다.
        /// </summary>
        /// <returns>플레이어 진영이면 true.</returns>
        public bool IsPlayerSide()
        {
            return _isPlayerSide;
        }

        /// <summary>
        /// 유닛 바디 리소스 ID를 반환한다.
        /// </summary>
        /// <returns>유닛 바디 ID 문자열.</returns>
        public string GetUnitBodyID()
        {
            return _unitBodyID;
        }

        /// <summary>
        /// 유닛 로직 ID를 반환한다.
        /// </summary>
        /// <returns>유닛 로직 ID 문자열.</returns>
        public string GetUnitLogicID()
        {
            return _unitLogicID;
        }

        /// <summary>
        /// 유닛에 경험치를 추가한다. 필요 경험치를 초과하면 자동으로 레벨업이 처리된다.
        /// </summary>
        /// <param name="exp">추가할 경험치 양.</param>
        public void AddExp(int exp)
        {
            _exp += exp;

            var requireExp = GetRequireExp();
            while (_exp >= requireExp)
            {
                _exp -= requireExp;

                SetLevel(_level + 1);
                LevelUpEvent?.Invoke(this, EventArgs.Empty);
                requireExp = _gameDataStore.GetRequireExp(_level);
                RequireExpChangedEvent?.Invoke(this, EventArgs.Empty);
            }

            ExpChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 유닛의 현재 경험치를 반환한다.
        /// </summary>
        /// <returns>현재 경험치 값.</returns>
        public int GetExp()
        {
            return _exp;
        }

        /// <summary>
        /// 현재 레벨에서 다음 레벨업에 필요한 경험치를 반환한다.
        /// </summary>
        /// <returns>필요 경험치 값.</returns>
        public int GetRequireExp()
        {
            return _gameDataStore.GetRequireExp(_level);
        }

        /// <summary>
        /// 유닛의 현재 레벨을 반환한다.
        /// </summary>
        /// <returns>현재 레벨 값.</returns>
        public int GetLevel()
        {
            return _level;
        }

        /// <summary>
        /// 유닛 바디 리소스 ID를 설정한다.
        /// </summary>
        /// <param name="id">설정할 유닛 바디 ID.</param>
        public void SetUnitBodyID(string id)
        {
            _unitBodyID = id;
        }

        /// <summary>
        /// 유닛 로직 ID를 설정한다.
        /// </summary>
        /// <param name="id">설정할 유닛 로직 ID.</param>
        public void SetUnitLogicID(string id)
        {
            _unitLogicID = id;
        }

        /// <summary>
        /// 유닛의 진영을 설정한다.
        /// </summary>
        /// <param name="isPlayerSide">플레이어 진영이면 true (기본값: true).</param>
        public void SetPlayerSide(bool isPlayerSide = true)
        {
            _isPlayerSide = isPlayerSide;
        }

        /// <summary>
        /// 유닛의 레벨을 설정하고 속성 중재자의 레벨도 함께 갱신한다.
        /// </summary>
        /// <param name="level">설정할 레벨 값.</param>
        public void SetLevel(int level)
        {
            _level = level;
            _attributeMediator.SetLevel(_level);
        }

        /// <summary>
        /// 유닛의 체력을 설정한다.
        /// </summary>
        /// <param name="hp">설정할 체력 값.</param>
        public void SetHP(int hp)
        {
            _hp = hp;
        }

        /// <summary>
        /// 활성 스킬을 유닛에 추가한다. 이미 동일 ID의 스킬이 있으면 무시한다.
        /// </summary>
        /// <param name="skill">추가할 활성 스킬.</param>
        public void AddSkill(IActiveSkill skill)
        {
            if (_skills.ContainsKey(skill.GetSkillID()))
                return;

            _skills.Add(skill.GetSkillID(), skill);
            SkillAddedEvent?.Invoke(this, new SkillAddedEventArgs { SkillID = skill.GetSkillID() });
        }

        /// <summary>
        /// 여러 속성 변경 효과를 유닛에 추가한다.
        /// </summary>
        /// <param name="effects">추가할 속성 변경 효과 컬렉션.</param>
        public void AddModifyAttributeEffects(IEnumerable<IModifyAttributeEffect> effects)
        {
            _attributeMediator.AddModifyAttributeEffects(effects);
        }

        /// <summary>
        /// 단일 속성 변경 효과를 유닛에 추가한다.
        /// </summary>
        /// <param name="effect">추가할 속성 변경 효과.</param>
        public void AddModifyAttributeEffect(IModifyAttributeEffect effect)
        {
            _attributeMediator.AddModifyAttributeEffect(effect);
        }

        /// <summary>
        /// 유닛이 보유한 스킬을 ID로 조회한다.
        /// </summary>
        /// <param name="skillID">조회할 스킬의 고유 식별자.</param>
        /// <returns>해당 <see cref="IActiveSkill"/> 인스턴스. 존재하지 않으면 null.</returns>
        public IActiveSkill GetSkill(string skillID)
        {
            return _skills.TryGetValue(skillID, out var skill) ? skill : null;
        }

        /// <summary>
        /// 유닛이 보유한 모든 활성 스킬을 반환한다.
        /// </summary>
        /// <returns>활성 스킬의 읽기 전용 컬렉션.</returns>
        public IReadOnlyCollection<IActiveSkill> GetSkills()
        {
            return _skills.Values;
        }

        /// <summary>
        /// 유닛의 위치를 설정한다.
        /// </summary>
        /// <param name="position">설정할 2D 위치 벡터.</param>
        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        /// <summary>
        /// 유닛의 현재 위치를 반환한다.
        /// </summary>
        /// <returns>현재 2D 위치 벡터.</returns>
        public Vector2 GetPosition()
        {
            return _position;
        }

        /// <summary>
        /// 유닛의 체력이 최대인지 확인한다.
        /// </summary>
        /// <returns>현재 체력이 최대 체력과 같으면 true.</returns>
        public bool IsFullHP()
        {
            return _hp == GetAttribute(AttributeID.MaxHP).AsInt();
        }

        /// <summary>
        /// 유닛에 치유를 적용한다. 최대 체력을 초과하지 않는다.
        /// </summary>
        /// <param name="healAmount">치유할 양.</param>
        public void ApplyHeal(int healAmount)
        {
            _hp = Math.Min(_hp + healAmount, GetAttribute(AttributeID.MaxHP).AsInt());
        }

        /// <summary>
        /// 유닛의 기본 스킬을 설정한다. 보유하지 않은 스킬이면 무시한다.
        /// </summary>
        /// <param name="skillID">기본 스킬로 설정할 스킬 ID.</param>
        public void SetDefaultSkill(string skillID)
        {
            if (!_skills.Any(entry => entry.Value.GetSkillID() == skillID))
                return;

            _defaultSkillID = skillID;
        }

        /// <summary>
        /// 유닛의 기본 스킬 ID를 반환한다.
        /// </summary>
        /// <returns>기본 스킬 ID 문자열.</returns>
        public string GetDefaultSkillID()
        {
            return _defaultSkillID;
        }

        /// <summary>
        /// 유닛을 사망 상태로 설정한다 (체력을 0으로 설정).
        /// </summary>
        public void SetDead()
        {
            SetHP(0);
        }

        /// <summary>
        /// 지정된 속성 변경 효과를 유닛에서 제거한다. (미구현)
        /// </summary>
        /// <param name="effects">제거할 속성 변경 효과 목록.</param>
        internal void RemoveModifyAttributeEffects(List<IModifyAttributeEffect> effects)
        {
            throw new NotImplementedException();
        }
    }
}
