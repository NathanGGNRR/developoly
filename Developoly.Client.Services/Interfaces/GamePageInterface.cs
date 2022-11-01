using Developoly.Client.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Developoly.Client.Services.Interfaces
{
    public interface GamePageInterface
    {
        General General { get; set; }

        void InitializePlayerPlayed(Object company);

        void MyDevelopperNotQualified(string DevsNotQualified);

        void MyTurnOver(string companyNext);

        void HisTurnOver(string companyNext);

        void MyDevAddSuccess(string AddedDev);

        void HisDevAddSuccess(string AddedDev);

        void MyDevAddNotEnoughMoney(string CantAddDev);

        void MyProjectAddSuccess(string AddedProject);

        void HisProjectAddSuccess(string AddedProject);

        void PlayerToPlay(string idCompany);

        void HisDevToCourseSuccess(string DevAndCourse);

    }
}