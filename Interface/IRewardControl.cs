using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IRewardControl
{
    public void Init(RewardModule[] rewardModules);

    public void UpdateRewardItemState();

    public void SetRewardItemWithPrizeItemInfo();
}
