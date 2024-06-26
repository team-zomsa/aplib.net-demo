// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

namespace Assets.Scripts.Items
{
    public class EndItem : Item
    {
        /// <summary>
        /// Uses the end item and triggers game over.
        /// </summary>
        public override void UseItem()
        {
            base.UseItem();

            GameManager.Instance.TriggerGameOver();
        }
    }
}
