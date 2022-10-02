using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Wonnasmith
{
    [Serializable]
    public class GOD_MODE_MANAGER : Singleton<GOD_MODE_MANAGER>
    {
        public delegate void GOD_MODE_GizmosClearButtonClick();
        public static event /*GOD_MODE_MANAGER.*/GOD_MODE_GizmosClearButtonClick GizmosClearButtonClick;

#if UNITY_EDITOR

        //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        #region "GOD_MODE_BUTTONS"
        [Serializable]
        public class God_Mode_Buttons
        {
            public KeyCode handOfGod_Button;
            public KeyCode seekerGismozClear_Button;
            public KeyCode conseoleClear_Button;
        }
        /********************************************************/
        [Header("===============GOD_MODE_BUTTONS===============")]
        /********************************************************/
        [Space(10)]
        public bool isButtonsActive;
        public God_Mode_Buttons god_Mode_Buttons;
        #endregion
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
        #region "HAND_OF_GOD"
        [Serializable]
        public class Hand_Of_God_Datas
        {
            public List<Hand> handList;

            [Serializable]
            public class Hand
            {
                public bool IS_HAND_ACTIVE;
                public GameObject prophet;
                public string functionName;
                public List<HandArgs> handArgsList;
            }

            [Serializable]
            public class HandArgs
            {
                object arg;
            }
        }

        /***************************************************/
        [Header("===============HAND_OF_GOD===============")]
        /***************************************************/
        [Space(10)]
        public Hand_Of_God_Datas hand_Of_God_Datas;
        #endregion
        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

        //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

        private void Update()
        {
            if (!isButtonsActive)
            {
                return;
            }

            Test_GismozClear(); // O
            Test_ConsoleClear(); // W
            Test_HandOfGod(); // 0
        }

        //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

        private void Test_GismozClear()
        {
            if (!Input.GetKeyDown(god_Mode_Buttons.seekerGismozClear_Button))
            {
                return;
            }

            GizmosClearButtonClick?.Invoke();

            Debug.Log("<color=green>:::WONNASMITH_IS_HERE <<[]>> GOD_MODE_GizmosClear:::</color>");
        }

        //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

        private void Test_ConsoleClear()
        {
            if (Input.GetKeyDown(god_Mode_Buttons.conseoleClear_Button))
            {
                var assembly = Assembly.GetAssembly(typeof(SceneView));
                var type = assembly.GetType("UnityEditor.LogEntries");
                var method = type.GetMethod("Clear");
                method.Invoke(new object(), null);

                Debug.Log("<color=red>:::WONNASMITH_IS_HERE <<[]>> GOD_MODE_ConsoleClear:::</color>");
            }
        }

        //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]

        private void Test_HandOfGod()
        {
            if (Input.GetKeyDown(god_Mode_Buttons.handOfGod_Button))
            {
                if (hand_Of_God_Datas == null)
                {
                    return;
                }

                if (hand_Of_God_Datas.handList == null)
                {
                    return;
                }

                for (int i = 0; i < hand_Of_God_Datas.handList.Count; i++)
                {
                    Hand_Of_God_Datas.Hand hand = hand_Of_God_Datas.handList[i];

                    if (hand.IS_HAND_ACTIVE)
                    {
                        if (hand.handArgsList == null)
                        {
                            hand.prophet.SendMessage(hand.functionName);
                        }
                        else
                        {
                            hand.prophet.SendMessage(hand.functionName, hand.handArgsList);
                        }

                        Debug.Log("<color=blue>:::WONNASMITH_IS_HERE <<[]>> GOD_MODE_Hand_Of_God::</color>" + "<color=black>" + hand.functionName + ":::</color>");
                    }
                }
            }
        }

        //[][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][][]
#endif
    }
}