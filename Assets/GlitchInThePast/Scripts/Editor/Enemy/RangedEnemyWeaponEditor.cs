using GlitchInThePast.Scripts.Player;
using Player.GenericMovement;
using Systems.Enemies;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(RangedEnemyWeapon))]
public class RangedEnemyWeaponEditor : Editor
{
    public VisualTreeAsset VisualTree;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        VisualTree.CloneTree(root);
        return root;
    }
}