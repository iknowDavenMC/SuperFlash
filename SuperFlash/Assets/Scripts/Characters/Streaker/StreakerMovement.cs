﻿using UnityEngine;
using System.Collections;

public class StreakerMovement : BaseMovement
{
		private static string S_SUPERFLASH_PARAMETER_NAME = "SuperFlash";
		private static int S_SUPERFLASH_PARAMETER_HASH;
		private static string S_IS_DANCING_PARAMETER_NAME = "IsDancing";
		private static int S_IS_DANCING_PARAMTER_HASH;

		protected override void UpdateState()
		{

		}

		protected override void UpdateMovementDirection()
		{
				AnimatorStateInfo currentAnimatorStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

				if (currentAnimatorStateInfo.tagHash == S_UNLOCKED_TAG_HASH)
				{
						HandleUserInput();
				}
		}

		private void HandleUserInput()
		{
				// Handle the user input
				m_movementDirection.x = Input.GetAxisRaw(UnityEditorConstants.S_HORIZONTAL_INPUT_AXIS);
				m_movementDirection.y = Input.GetAxisRaw(UnityEditorConstants.S_VERTICAL_INPUT_AXIS);
				m_movementDirection.Normalize();

				// Handle the animation update
				SetIsMoving(m_movementDirection != Vector2.zero);
		}
}