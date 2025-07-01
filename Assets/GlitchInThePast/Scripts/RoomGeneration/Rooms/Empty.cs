namespace GlitchInThePast.Scripts.RoomGeneration
{
    public class Empty : Room
    {
        public override void ResetRoom()
        {
            roomNameText.text = "Empty Room";
        }
    }
}