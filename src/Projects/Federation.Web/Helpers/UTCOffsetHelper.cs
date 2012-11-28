using System.Collections.Generic;
using System.Web.Mvc;

namespace Federation.Core
{
    public static class UTCOffsetHelper
    {
        public static IList<SelectListItem> SelectListItems
        {
            get { return _list; }
        }

        private static IList<SelectListItem> _list = new List<SelectListItem>();

        static UTCOffsetHelper()
        {
            _list.Add(new SelectListItem { Value = "-11:00", Text = "(UTC-11:00) Самоа" });
            _list.Add(new SelectListItem { Value = "-10:00", Text = "(UTC-10:00) Гавайи" });
            _list.Add(new SelectListItem { Value = "-9:00", Text = "(UTC-9:00) Аляска" });
            _list.Add(new SelectListItem { Value = "-8:00", Text = "(UTC-8:00) Нижняя Калифорния" });
            _list.Add(new SelectListItem { Value = "-8:00", Text = "(UTC-8:00) Тихоокеанское время (США и Канада)" });
            _list.Add(new SelectListItem { Value = "-7:00", Text = "(UTC-7:00) Аризона" });
            _list.Add(new SelectListItem { Value = "-7:00", Text = "(UTC-7:00) Горное время (США и Канада)" });
            _list.Add(new SelectListItem { Value = "-7:00", Text = "(UTC-7:00) Ла-Пас, Мазатлан, Чихуахуа" });
            _list.Add(new SelectListItem { Value = "-6:00", Text = "(UTC-6:00) Гвадалахара, Мехико, Монтеррей" });
            _list.Add(new SelectListItem { Value = "-6:00", Text = "(UTC-6:00) Саскачеван" });
            _list.Add(new SelectListItem { Value = "-6:00", Text = "(UTC-6:00) Центральная Америка" });
            _list.Add(new SelectListItem { Value = "-6:00", Text = "(UTC-6:00) Центральное время (США и Канада)" });
            _list.Add(new SelectListItem { Value = "-5:00", Text = "(UTC-5:00) Богота, Кито, Лима" });
            _list.Add(new SelectListItem { Value = "-5:00", Text = "(UTC-5:00) Восточное время (США и Канада)" });
            _list.Add(new SelectListItem { Value = "-5:00", Text = "(UTC-5:00) Индиана (восток)" });
            _list.Add(new SelectListItem { Value = "-4:30", Text = "(UTC-4:30) Каракас" });
            _list.Add(new SelectListItem { Value = "-4:00", Text = "(UTC-4:00) Асунсьон" });
            _list.Add(new SelectListItem { Value = "-4:00", Text = "(UTC-4:00) Атлантическое время (Канада)" });
            _list.Add(new SelectListItem { Value = "-4:00", Text = "(UTC-4:00) Джорджтаун, Ла-Пас, Манаус, Сан-Хуан" });
            _list.Add(new SelectListItem { Value = "-4:00", Text = "(UTC-4:00) Куяба" });
            _list.Add(new SelectListItem { Value = "-4:00", Text = "(UTC-4:00) Сантьяго" });
            _list.Add(new SelectListItem { Value = "-3:30", Text = "(UTC-3:30) Ньюфаундленд" });
            _list.Add(new SelectListItem { Value = "-3:00", Text = "(UTC-3:00) Бразилия" });
            _list.Add(new SelectListItem { Value = "-3:00", Text = "(UTC-3:00) Буэнос-Айрес" });
            _list.Add(new SelectListItem { Value = "-3:00", Text = "(UTC-3:00) Гренландия" });
            _list.Add(new SelectListItem { Value = "-3:00", Text = "(UTC-3:00) Кайенна, Форталеза" });
            _list.Add(new SelectListItem { Value = "-3:00", Text = "(UTC-3:00) Монтевидео" });
            _list.Add(new SelectListItem { Value = "-2:00", Text = "(UTC-2:00) Среднеатлантическое время" });
            _list.Add(new SelectListItem { Value = "-1:00", Text = "(UTC-1:00) Азорские о-ва" });
            _list.Add(new SelectListItem { Value = "-1:00", Text = "(UTC-1:00) О-ва Зеленого Мыса" });
            _list.Add(new SelectListItem { Value = "0:00", Text = "(UTC+0:00) Дублин, Лиссабон, Лондон, Эдинбург" });
            _list.Add(new SelectListItem { Value = "0:00", Text = "(UTC+0:00) Касабланка" });
            _list.Add(new SelectListItem { Value = "0:00", Text = "(UTC+0:00) Монровия, Рейкьявик" });
            _list.Add(new SelectListItem { Value = "1:00", Text = "(UTC+1:00) Амстердам, Берлин, Берн, Вена, Рим, Стокгольм" });
            _list.Add(new SelectListItem { Value = "1:00", Text = "(UTC+1:00) Белград, Братислава, Будапешт, Любляна, Прага" });
            _list.Add(new SelectListItem { Value = "1:00", Text = "(UTC+1:00) Брюссель, Копенгаген, Мадрид, Париж" });
            _list.Add(new SelectListItem { Value = "1:00", Text = "(UTC+1:00) Варшава, Загреб, Сараево, Скопье" });
            _list.Add(new SelectListItem { Value = "1:00", Text = "(UTC+1:00) Виндук" });
            _list.Add(new SelectListItem { Value = "1:00", Text = "(UTC+1:00) Западная Центральная Африка" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Амман" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Афины, Бухарест" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Бейрут" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Вильнюс, Киев, Рига, София, Таллин, Хельсинки" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Дамаск" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Иерусалим" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Каир" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Минск" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Стамбул" });
            _list.Add(new SelectListItem { Value = "2:00", Text = "(UTC+2:00) Хараре, Претония" });
            _list.Add(new SelectListItem { Value = "3:00", Text = "(UTC+3:00) Багдад" });
            _list.Add(new SelectListItem { Value = "3:00", Text = "(UTC+3:00) Калининград" });
            _list.Add(new SelectListItem { Value = "3:00", Text = "(UTC+3:00) Кувейт, Эр-Рияд" });
            _list.Add(new SelectListItem { Value = "3:00", Text = "(UTC+3:00) Найроби" });
            _list.Add(new SelectListItem { Value = "3:30", Text = "(UTC+3:30) Тегеран" });
            _list.Add(new SelectListItem { Value = "4:00", Text = "(UTC+4:00) Абу-Даби, Мускат" });
            _list.Add(new SelectListItem { Value = "4:00", Text = "(UTC+4:00) Баку" });
            _list.Add(new SelectListItem { Value = "4:00", Text = "(UTC+4:00) Волгоград, Москва, Санкт-Петербург" });
            _list.Add(new SelectListItem { Value = "4:00", Text = "(UTC+4:00) Ереван" });
            _list.Add(new SelectListItem { Value = "4:00", Text = "(UTC+4:00) Порт-Луи" });
            _list.Add(new SelectListItem { Value = "4:00", Text = "(UTC+4:00) Тбилиси" });
            _list.Add(new SelectListItem { Value = "4:30", Text = "(UTC+4:30) Кабул" });
            _list.Add(new SelectListItem { Value = "5:00", Text = "(UTC+5:00) Исламабад, Карачи" });
            _list.Add(new SelectListItem { Value = "5:00", Text = "(UTC+5:00) Ташкент" });
            _list.Add(new SelectListItem { Value = "5:30", Text = "(UTC+5:30) Колката, Мумбаи, Нью-Дели, Ченнай" });
            _list.Add(new SelectListItem { Value = "5:30", Text = "(UTC+5:30) Шри-Джаявардене-пура-Котте" });
            _list.Add(new SelectListItem { Value = "5:45", Text = "(UTC+5:45) Катманду" });
            _list.Add(new SelectListItem { Value = "6:00", Text = "(UTC+6:00) Астана" });
            _list.Add(new SelectListItem { Value = "6:00", Text = "(UTC+6:00) Дакка" });
            _list.Add(new SelectListItem { Value = "6:00", Text = "(UTC+6:00) Екатеринбург" });
            _list.Add(new SelectListItem { Value = "6:30", Text = "(UTC+6:30) Янгон" });
            _list.Add(new SelectListItem { Value = "7:00", Text = "(UTC+7:00) Бангкок, Джакарта, Ханой" });
            _list.Add(new SelectListItem { Value = "7:00", Text = "(UTC+7:00) Новосибирск" });
            _list.Add(new SelectListItem { Value = "8:00", Text = "(UTC+8:00) Гонконг, Пекин, Урумчи, Чунцин" });
            _list.Add(new SelectListItem { Value = "8:00", Text = "(UTC+8:00) Красноярск" });
            _list.Add(new SelectListItem { Value = "8:00", Text = "(UTC+8:00) Куала-Лумпур, Сингапур" });
            _list.Add(new SelectListItem { Value = "8:00", Text = "(UTC+8:00) Перт" });
            _list.Add(new SelectListItem { Value = "8:00", Text = "(UTC+8:00) Тайбэй" });
            _list.Add(new SelectListItem { Value = "8:00", Text = "(UTC+8:00) Улан-Батор" });
            _list.Add(new SelectListItem { Value = "9:00", Text = "(UTC+9:00) Иркутск" });
            _list.Add(new SelectListItem { Value = "9:00", Text = "(UTC+9:00) Осака, Саппоро, Токио" });
            _list.Add(new SelectListItem { Value = "9:00", Text = "(UTC+9:00) Сеул" });
            _list.Add(new SelectListItem { Value = "9:30", Text = "(UTC+9:30) Аделаида" });
            _list.Add(new SelectListItem { Value = "9:30", Text = "(UTC+9:30) Дарвин" });
            _list.Add(new SelectListItem { Value = "10:00", Text = "(UTC+10:00) Брисбен" });
            _list.Add(new SelectListItem { Value = "10:00", Text = "(UTC+10:00) Гуам, Порт-Морсби" });
            _list.Add(new SelectListItem { Value = "10:00", Text = "(UTC+10:00) Канберра, Мельбурн, Сидней" });
            _list.Add(new SelectListItem { Value = "10:00", Text = "(UTC+10:00) Хобарт" });
            _list.Add(new SelectListItem { Value = "10:00", Text = "(UTC+10:00) Якутск" });
            _list.Add(new SelectListItem { Value = "11:00", Text = "(UTC+11:00) Владивосток" });
            _list.Add(new SelectListItem { Value = "11:00", Text = "(UTC+11:00) Соломоновы о-ва, Нов. Каледония" });
            _list.Add(new SelectListItem { Value = "12:00", Text = "(UTC+12:00) Веллингтон, Окленд" });
            _list.Add(new SelectListItem { Value = "12:00", Text = "(UTC+12:00) Магадан" });
            _list.Add(new SelectListItem { Value = "12:00", Text = "(UTC+12:00) Фиджи" });
            _list.Add(new SelectListItem { Value = "13:00", Text = "(UTC+13:00) Нукуалофа" });
        }
    }
}