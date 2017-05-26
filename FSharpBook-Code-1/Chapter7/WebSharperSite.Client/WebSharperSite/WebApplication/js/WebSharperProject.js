// p135adb51f93b5522

if (typeof WebSharperProject$ == "undefined")
{
  WebSharperProject$ = {};
}

WebSharperProject$.$MainPage =
function ()
{};

(WebSharperProject$.$MainPage).prototype = new W$.$Control();

((WebSharperProject$.$MainPage).prototype).get_Body =
function (unitVar1)
{
  var _this = this;
  var x = new W$.$List({
                         $: 1,
                         $0: ((W$.Html).text)(""),
                         $1: new W$.$List({
                                            $: 0
                                          })
                       });
  var body = x;
  var name = "p";
  var result = ((W$.Html).Tag)(name, body);
  var x_2 = new W$.$List({
                           $: 1,
                           $0: ((W$.Html).text)("Press to retrieve data"),
                           $1: new W$.$List({
                                              $: 0
                                            })
                         });
  var body_1 = x_2;
  var name_1 = "p";
  var x_4 = "Button";
  var x_5 = "Get Data";
  var x_3 =
      new W$.$List({
                     $: 1,
                     $0: ((W$.Attr).NewAttr)("type", x_4),
                     $1: new W$.$List({
                                        $: 1,
                                        $0: ((W$.Attr).NewAttr)("value", x_5),
                                        $1: new W$.$List({
                                                           $: 0
                                                         })
                                      })
                   });
  var body_2 = x_3;
  var name_2 = "input";
  var f =
      function (e)
      {
        return function (a)
               {
                 return (result.set_Text)(
                        ((WebSharperProject$.Application$).GetPrimes)());
               };
      };
  var x_1 =
      new W$.$List({
                     $: 1,
                     $0: ((W$.Html).Tag)(name_1, body_1),
                     $1:
                     new W$.$List({
                                    $: 1,
                                    $0:
                                    (((W$.Html).Events).op_BarGreaterBang)(
                                    ((W$.Html).Tag)(name_2, body_2),
                                    function (t)
                                    {
                                      var t_1 = t;
                                      var f_1 = f;
                                      return (((W$.Html).Events).On)(
                                             "click", f_1, t_1);
                                    }),
                                    $1: new W$.$List({
                                                       $: 1,
                                                       $0: result,
                                                       $1: new W$.$List({
                                                                          $: 0
                                                                        })
                                                     })
                                  })
                   });
  var body_3 = x_1;
  var name_3 = "div";
  return ((W$.Html).Tag)(name_3, body_3);
};

if (typeof WebSharperProject$.Application$ == "undefined")
{
  WebSharperProject$.Application$ = {};
}

(WebSharperProject$.Application$).primes =
function (_arg1)
{
  var __1;
  if (_arg1.$ == 1)
  {
    var t = _arg1.$1;
    var h = _arg1.$0;
    var l = t;
    var p = function (x)
            {
              return x % h > 0;
            };
    __1 =
    new W$.$List({
                   $: 1,
                   $0: h,
                   $1:
                   ((WebSharperProject$.Application$).primes)(
                   ((W$.Seq$).toList)(((W$.Seq$).filter)(p, l)))
                 });
  }
  else
  {
    __1 = new W$.$List({
                         $: 0
                       });
  }
  return __1;
};

(WebSharperProject$.Application$).GetPrimes =
function (unitVar0)
{
  var folder = function (s)
               {
                 return function (i)
                        {
                          return (s + " ") + (i.toString)();
                        };
               };
  var state = "";
  return (function (list)
          {
            var l = list;
            var s = state;
            var f = folder;
            return ((W$.Seq$).fold)(f, s, l);
          })(
         ((WebSharperProject$.Application$).primes)(
         ((W$.Seq$).toList)(((W$.Op$).Range)(2, 100))));
};

((((W$.Registry).Types).get)()).MainPage_n624d6a77cbc87721 =
WebSharperProject$.$MainPage;
((WebSharperProject$.$MainPage).prototype).$$IComponent_p4642b924f95be4a6__IComponent
= 1;
((WebSharperProject$.$MainPage).prototype).$$IDisposable_p1a5dece2430d1c7__IDisposable
= 1;
((WebSharperProject$.$MainPage).prototype).$$IParserAccessor_p23114829d1dcea79__IParserAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IUrlResolutionService_p3d8fd69f2d090451__IUrlResolutionService
= 1;
((WebSharperProject$.$MainPage).prototype).$$IDataBindingsAccessor_naf467a036212eec__IDataBindingsAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IControlBuilderAccessor_p12767bfcffc685b4__IControlBuilderAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IControlDesignerAccessor_p21c9dfd732eeac7e__IControlDesignerAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IExpressionsAccessor_p63d69e724f7de2af__IExpressionsAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IWidget_p4036373d8372c14d__IWidget
= 1;
((WebSharperProject$.$MainPage).prototype).$$INode_p5129303de1c6d8ef__INode = 1;
