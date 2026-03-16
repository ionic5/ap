using System;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    /// <summary>
    /// 에셋 ID와 프리팹 게임오브젝트를 매핑하는 직렬화 가능한 컨테이너 클래스.
    /// Unity 인스펙터에서 에셋 ID별 프리팹을 설정할 때 사용된다.
    /// </summary>
    [Serializable]
    public class PrefabContainer
    {
        /// <summary>프리팹에 대응하는 에셋 식별자</summary>
        [SerializeField]
        public string AssetID;
        /// <summary>인스턴스화할 프리팹 게임오브젝트</summary>
        [SerializeField]
        public GameObject Prefab;
    }
}
