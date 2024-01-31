using Interfaces;
using MaxisGeneralPurpose.Event;
using TMPro;
using UnityEngine;

namespace MaxisGeneralPurpose
{
    public class TextWriterFromDataCarrier : MonoBehaviour
    {
        [SerializeField] private DataCarrier value;

        private TMP_Text _textMesh;

        [SerializeField] private string postFix;


        private void Awake()
        {
            if (!_textMesh)
            {
                _textMesh = GetComponent<TMP_Text>();
            }

            OnEventRaised();
        }

        private void OnEnable()
        {
            value.raiseOnValueChanged.RegisterListener(OnEventRaised);
        }

        private void OnDisable()
        {
            value.raiseOnValueChanged.UnregisterListener(OnEventRaised);
        }

        public void OnEventRaised()
        {
            _textMesh.text = value + postFix;
        }
    }
}