using OkoCommon.Communication;
using OkoCommon.Game;

namespace Tests;

public class MiscTests
{
    [Test]
    public void CorrectCast()
    {
        var notif = Notification.Create(NotifEnum.AskName, new MockPlayer("test", 1000).Id);

        if (notif is Notification<Guid> objNotif)
            Assert.Pass();
        else
            Assert.Fail();
    }
}