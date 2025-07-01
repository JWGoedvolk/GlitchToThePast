namespace GlitchInThePast.Scripts.RoomGeneration
{
    public class Boss :  Room
    {
        public override void ResetRoom()
        {
            roomNameText.text = "Boss Room";
        }
    }
}