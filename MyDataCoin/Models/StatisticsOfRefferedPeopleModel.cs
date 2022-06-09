using System;
namespace MyDataCoin.Models
{
    public class StatisticsOfRefferedPeopleModel
    {
        public StatisticsOfRefferedPeopleModel(int refferedPeople, double rewardsAmount)
        {
            RefferedPeople = refferedPeople;
            RewardsAmount = rewardsAmount;
        }

        public int RefferedPeople { get; set; }

        public double RewardsAmount { get; set; }
    }
}
