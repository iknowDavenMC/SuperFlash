using System;

using UnityEngine;
using Assets.Code._XNA;

namespace COMP476Proj
{
    static class Program : MonoBehaviour
    {
				SuperFlashGame m_game;

				void Awake()
				{
						m_game = new SuperFlashGame();

						m_game.Run();
        }
    }
}

