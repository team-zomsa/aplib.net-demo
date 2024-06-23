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
