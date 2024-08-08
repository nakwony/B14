using UnityEngine;

namespace Quest.Core.Reward
{
    public abstract class Reward : ScriptableObject
    {
        [SerializeField]
        private Sprite _icon;
        [SerializeField]
        private string _description;
        [SerializeField]
        private int _quantity;

        public Sprite Icon => _icon;
        public string Description => _description;
        protected int Quantity => _quantity;

        public abstract void Give(Quest quest);
    }
}
