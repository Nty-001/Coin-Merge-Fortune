using UnityEngine;
using UnityEngine.UI;
namespace CoinMerge.Recovery
{
    // Authored Image groups reuse the country's approved single banknote without raster resampling.
    [DisallowMultipleComponent, RequireComponent(typeof(Image))]
    public sealed class RecoveredCurrencyArtwork : MonoBehaviour
    {
        public Image single;
        public GameObject smallPile, largePile;
        public Image[] smallNotes, largeNotes;
        public int CurrentType { get; private set; }
        public void Apply(Sprite banknote, int type)
        {
            CurrentType = type;
            single.sprite = banknote;
            single.preserveAspect = true;
            single.enabled = type == 1;
            smallPile.SetActive(type == 2);
            largePile.SetActive(type == 3);
            var notes = type == 2 ? smallNotes : largeNotes;
            if (type > 1) foreach (var note in notes) note.sprite = banknote;
        }
    }
}
