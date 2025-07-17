using GlitchInThePast.Scripts.Player;
using JW.Roguelike.Objects.Interactibles;
using Player.GenericMovement;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(CoopButtonsInteractible))]
public class CoopButtonsInteractibleEditor : Editor
{
    public VisualTreeAsset VisualTree;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        VisualTree.CloneTree(root);
        return root;
    }
}